// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.EnumerableExtensions;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public abstract class RemoteExpressionVisitorBase
    {
        [return: NotNullIfNotNull("node")]
        protected virtual Expression? Visit(Expression? node)
            => node?.NodeType switch
            {
                null => null,
                ExpressionType.Binary => VisitBinary((BinaryExpression)node),
                ExpressionType.Block => VisitBlock((BlockExpression)node),
                ExpressionType.Call => VisitMethodCall((MethodCallExpression)node),
                ExpressionType.Conditional => VisitConditional((ConditionalExpression)node),
                ExpressionType.Constant => VisitConstant((ConstantExpression)node),
                ExpressionType.Default => VisitDefault((DefaultExpression)node),
                ExpressionType.Goto => VisitGoto((GotoExpression)node),
                ExpressionType.Invoke => VisitInvoke((InvokeExpression)node),
                ExpressionType.Label => VisitLabel((LabelExpression)node),
                ExpressionType.Lambda => VisitLambda((LambdaExpression)node),
                ExpressionType.ListInit => VisitListInit((ListInitExpression)node),
                ExpressionType.Loop => VisitLoop((LoopExpression)node),
                ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)node),
                ExpressionType.MemberInit => VisitMemberInit((MemberInitExpression)node),
                ExpressionType.New => VisitNew((NewExpression)node),
                ExpressionType.NewArray => VisitNewArray((NewArrayExpression)node),
                ExpressionType.Parameter => VisitParameter((ParameterExpression)node),
                ExpressionType.Switch => VisitSwitch((SwitchExpression)node),
                ExpressionType.TypeIs => VisitTypeIs((TypeBinaryExpression)node),
                ExpressionType.Try => VisitTry((TryExpression)node),
                ExpressionType.Unary => VisitUnary((UnaryExpression)node),
                _ => throw new NotSupportedException($"Unknown expression type: '{node.NodeType}'"),
            };

        protected virtual Expression VisitSwitch(SwitchExpression node)
        {
            var defaultExpression = Visit(node.CheckNotNull(nameof(node)).DefaultExpression);
            var switchValue = Visit(node.SwitchValue);
            var cases = node.Cases.AsEmptyIfNull().Select(VisitSwitchCase).ToList();
            if (defaultExpression != node.DefaultExpression ||
                switchValue != node.SwitchValue ||
                !cases.SequenceEqual(node.Cases))
            {
                return new SwitchExpression(switchValue, node.Comparison, defaultExpression, cases);
            }

            return node;
        }

        protected virtual SwitchCase VisitSwitchCase(SwitchCase switchCase)
        {
            var body = Visit(switchCase.CheckNotNull(nameof(switchCase)).Body);
            var testValues = switchCase.TestValues.AsEmptyIfNull();
            var newTestValues = testValues.Select(Visit).ToList();
            if (body != switchCase.Body || !testValues.SequenceEqual(newTestValues))
            {
                return new SwitchCase(body, newTestValues!);
            }

            return switchCase;
        }

        protected virtual Expression VisitTry(TryExpression node)
        {
            var @finally = Visit(node.CheckNotNull(nameof(node)).Finally);
            var body = Visit(node.Body);
            var fault = Visit(node.Fault);
            var handlers = node.Handlers.AsEmptyIfNull().Select(VisitCatchBlock).ToList();
            if (@finally != node.Finally ||
                body != node.Body ||
                fault != node.Fault ||
                !handlers.SequenceEqual(node.Handlers))
            {
                return new TryExpression(node.Type, body, fault, @finally, handlers);
            }

            return node;
        }

        protected virtual CatchBlock VisitCatchBlock(CatchBlock catchBlock)
        {
            var body = Visit(catchBlock.CheckNotNull(nameof(catchBlock)).Body);
            var variable = catchBlock.Variable is null ? null : VisitParameter(catchBlock.Variable);
            var filter = Visit(catchBlock.Filter);
            if (body != catchBlock.Body || variable != catchBlock.Variable || filter != catchBlock.Filter)
            {
                return new CatchBlock(catchBlock.Test, variable, body, filter);
            }

            return catchBlock;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            var n = VisitNew(node.CheckNotNull(nameof(node)).NewExpression);
            var bindings = VisitBindingList(node.Bindings);

            if (n != node.NewExpression || bindings != node.Bindings)
            {
                return new MemberInitExpression(n, bindings);
            }

            return node;
        }

        [return: NotNullIfNotNull("list")]
        protected virtual List<MemberBinding>? VisitBindingList(List<MemberBinding>? list)
        {
            if (list is null)
            {
                return list;
            }

            List<MemberBinding>? visited = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                MemberBinding b = VisitBinding(list[i]);
                if (visited is not null)
                {
                    visited.Add(b);
                }
                else if (b != list[i])
                {
                    visited = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        visited.Add(list[j]);
                    }

                    visited.Add(b);
                }
            }

            return visited ?? list;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
            => binding.CheckNotNull(nameof(binding)).BindingType switch
            {
                MemberBindingType.Assignment => VisitMemberAssignment((MemberAssignment)binding),
                MemberBindingType.MemberBinding => VisitMemberMemberBinding((MemberMemberBinding)binding),
                MemberBindingType.ListBinding => VisitMemberListBinding((MemberListBinding)binding),
                _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
            };

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.CheckNotNull(nameof(assignment)).Expression);
            if (e != assignment.Expression)
            {
                return new MemberAssignment(assignment.Member, e);
            }

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.CheckNotNull(nameof(binding)).Bindings.ToList());
            if (bindings != binding.Bindings)
            {
                return new MemberMemberBinding(binding.Member, bindings);
            }

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.CheckNotNull(nameof(binding)).Initializers);
            if (!ReferenceEquals(initializers, binding.Initializers))
            {
                return new MemberListBinding(binding.Member, initializers);
            }

            return binding;
        }

        [return: NotNullIfNotNull("list")]
        protected virtual List<ElementInit>? VisitElementInitializerList(List<ElementInit>? list)
        {
            if (list is null)
            {
                return list;
            }

            List<ElementInit>? visited = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                var init = VisitElementInitializer(list[i]);
                if (visited is not null)
                {
                    visited.Add(init);
                }
                else if (init != list[i])
                {
                    visited = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        visited.Add(list[j]);
                    }

                    visited.Add(init);
                }
            }

            return visited ?? list;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.CheckNotNull(nameof(initializer)).Arguments);
            if (!ReferenceEquals(arguments, initializer.Arguments))
            {
                return new ElementInit(initializer.AddMethod.ToMethodInfo(), arguments);
            }

            return initializer;
        }

        [return: NotNullIfNotNull("list")]
        protected virtual List<T>? VisitExpressionList<T>(List<T>? list)
            where T : Expression
        {
            if (list is null)
            {
                return list;
            }

            List<T>? visited = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                var p = (T)Visit(list[i]);
                if (visited is not null)
                {
                    visited.Add(p);
                }
                else if (p != list[i])
                {
                    visited = new List<T>(n);
                    for (int j = 0; j < i; j++)
                    {
                        visited.Add(list[j]);
                    }

                    visited.Add(p);
                }
            }

            return visited ?? list;
        }

        protected virtual NewExpression VisitNew(NewExpression node)
        {
            var args = VisitExpressionList(node.CheckNotNull(nameof(node)).Arguments);
            if (!ReferenceEquals(args, node.Arguments))
            {
                return node.Constructor is null
                    ? new NewExpression(node.Type ?? throw new RemoteLinqException($"{nameof(NewExpression)} requires either {nameof(NewExpression.Type)} or {nameof(NewExpression.Constructor)} property not null."))
                    : new NewExpression(node.Constructor, args, node.Members);
            }

            return node;
        }

        private Expression VisitNewArray(NewArrayExpression node)
        {
            var expressions = VisitExpressionList(node.Expressions);
            if (!ReferenceEquals(expressions, node.Expressions))
            {
                return new NewArrayExpression(node.NewArrayType, node.Type, expressions);
            }

            return node;
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            var newExpression = VisitNew(node.CheckNotNull(nameof(node)).NewExpression);
            if (!ReferenceEquals(newExpression, node.NewExpression))
            {
                return new ListInitExpression(newExpression, node.Initializers);
            }

            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            var leftOperand = Visit(node.CheckNotNull(nameof(node)).LeftOperand);
            var rightOperand = Visit(node.RightOperand);
            var conversion = Visit(node.Conversion) as LambdaExpression;
            if (!ReferenceEquals(leftOperand, node.LeftOperand) || !ReferenceEquals(rightOperand, node.RightOperand) || !ReferenceEquals(conversion, node.Conversion))
            {
                return new BinaryExpression(node.BinaryOperator, leftOperand, rightOperand, node.IsLiftedToNull, node.Method, conversion);
            }

            return node;
        }

        protected virtual Expression VisitInvoke(InvokeExpression node)
        {
            var expression = Visit(node.CheckNotNull(nameof(node)).Expression);
            var arguments = VisitExpressionList(node.Arguments);
            if (!ReferenceEquals(expression, node.Expression) || !ReferenceEquals(arguments, node.Arguments))
            {
                return new InvokeExpression(expression, arguments);
            }

            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node)
        {
            var variables = VisitExpressionList(node.CheckNotNull(nameof(node)).Variables);
            var expressions = VisitExpressionList(node.Expressions);
            if (!ReferenceEquals(variables, node.Variables) || !ReferenceEquals(expressions, node.Expressions))
            {
                return new BlockExpression(node.Type, variables, expressions);
            }

            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            var exp = Visit(node.CheckNotNull(nameof(node)).Expression);
            if (!ReferenceEquals(exp, node.Expression))
            {
                return new TypeBinaryExpression(exp, node.TypeOperand);
            }

            return node;
        }

        protected virtual Expression VisitConditional(ConditionalExpression node)
        {
            var test = Visit(node.CheckNotNull(nameof(node)).Test);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);
            if (!ReferenceEquals(test, node.Test) || !ReferenceEquals(ifTrue, node.IfTrue) || !ReferenceEquals(ifFalse, node.IfFalse))
            {
                return new ConditionalExpression(test, ifTrue, ifFalse);
            }

            return node;
        }

        protected virtual ConstantExpression VisitConstant(ConstantExpression node)
            => node;

        protected virtual ParameterExpression VisitParameter(ParameterExpression node)
            => node;

        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            var instance = Visit(node.CheckNotNull(nameof(node)).Expression);
            if (!ReferenceEquals(instance, node.Expression))
            {
                return new MemberExpression(instance, node.Member);
            }

            return node;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.CheckNotNull(nameof(node)).Operand);
            if (!ReferenceEquals(operand, node.Operand))
            {
                return new UnaryExpression(node.UnaryOperator, operand, node.Type, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var instance = Visit(node.CheckNotNull(nameof(node)).Instance);
            var argumements = node.Arguments?
                .Select(i => new { Old = i, New = Visit(i) })
                .ToList();
            if (!ReferenceEquals(instance, node.Instance) || argumements?.Any(i => !ReferenceEquals(i.Old, i.New)) is true)
            {
                return new MethodCallExpression(instance, node.Method, argumements.Select(i => i.New));
            }

            return node;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            var exp = Visit(node.CheckNotNull(nameof(node)).Expression);
            var parameters = node.Parameters?
                .Select(i => new { Old = i, New = VisitParameter(i) })
                .ToList();
            if (!ReferenceEquals(exp, node.Expression) || parameters?.Any(i => !ReferenceEquals(i.Old, i.New)) is true)
            {
                return new LambdaExpression(node.Type, exp, parameters?.Select(i => i.New));
            }

            return node;
        }

        protected virtual Expression VisitDefault(DefaultExpression node)
            => node;

        protected virtual Expression VisitGoto(GotoExpression node)
        {
            var value = Visit(node.CheckNotNull(nameof(node)).Value);
            if (!ReferenceEquals(value, node.Value))
            {
                return new GotoExpression(node.Kind, node.Target, node.Type, node.Value);
            }

            return node;
        }

        protected virtual Expression VisitLabel(LabelExpression node)
        {
            var defaultValue = Visit(node.CheckNotNull(nameof(node)).DefaultValue);
            if (!ReferenceEquals(defaultValue, node.DefaultValue))
            {
                return new LabelExpression(node.Target, defaultValue);
            }

            return node;
        }

        protected virtual LoopExpression VisitLoop(LoopExpression node)
        {
            var body = Visit(node.CheckNotNull(nameof(node)).Body);
            if (!ReferenceEquals(body, node.Body))
            {
                return new LoopExpression(body, node.BreakLabel, node.ContinueLabel);
            }

            return node;
        }
    }
}
