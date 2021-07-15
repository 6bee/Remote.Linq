// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// From http://msdn.microsoft.com/en-us/library/bb882521.aspx.
    /// </summary>
    /// <remarks>
    /// In this expression tree visitor implementation, the Visit method, which should be called first,
    /// dispatches the expression it is passed to one of the more specialized visitor methods in the class,
    /// based on the type of the expression. The specialized visitor methods visit the sub-tree of the
    /// expression they are passed. If a sub-expression changes after it has been visited, for example by
    /// an overriding method in a derived class, the specialized visitor methods create a new expression
    /// that includes the changes in the sub-tree. Otherwise, they return the expression that they were passed.
    /// This recursive behavior enables a new expression tree to be built that either is the same as or a
    /// modified version of the original expression that was passed to Visit.
    /// </remarks>
    public abstract class SystemExpressionVisitorBase
    {
        [return: NotNullIfNotNull("node")]
        protected virtual Expression? Visit(Expression? node)
            => node switch
            {
                null => null,
                UnaryExpression unaryExpression => VisitUnary(unaryExpression),
                BinaryExpression binaryExpression => VisitBinary(binaryExpression),
                NewArrayExpression newArrayExpression => VisitNewArray(newArrayExpression),
                BlockExpression blockExpression => VisitBlock(blockExpression),
                MethodCallExpression methodCallExpression => VisitMethodCall(methodCallExpression),
                ConditionalExpression conditionalExpression => VisitConditional(conditionalExpression),
                ConstantExpression constantExpression => VisitConstant(constantExpression),
                DefaultExpression defaultExpression => VisitDefault(defaultExpression),
                GotoExpression gotoExpression => VisitGoto(gotoExpression),
                InvocationExpression invocationExpression => VisitInvocation(invocationExpression),
                LabelExpression labelExpression => VisitLabel(labelExpression),
                LambdaExpression lambdaExpression => VisitLambda(lambdaExpression),
                ListInitExpression listInitExpression => VisitListInit(listInitExpression),
                LoopExpression loopExpression => VisitLoop(loopExpression),
                MemberExpression memberExpression => VisitMemberAccess(memberExpression),
                MemberInitExpression memberInitExpression => VisitMemberInit(memberInitExpression),
                NewExpression newExpression => VisitNew(newExpression),
                ParameterExpression parameterExpression => VisitParameter(parameterExpression),
                SwitchExpression switchExpression => VisitSwitch(switchExpression),
                TryExpression tryExpression => VisitTry(tryExpression),
                TypeBinaryExpression typeBinaryExpression => VisitTypeIs(typeBinaryExpression),
#pragma warning disable CA1062 // Validate arguments of public methods --> as this is handled by switch expression
                _ => throw new RemoteLinqException($"Unhandled expression type: '{node.NodeType}'"),
#pragma warning restore CA1062 // Validate arguments of public methods
            };

        protected virtual Expression VisitSwitch(SwitchExpression node)
        {
            var cases = (node.CheckNotNull(nameof(node)).Cases ?? Enumerable.Empty<SwitchCase>()).ToList();
            var switchCases = cases.Select(VisitSwitchCase).ToList();
            var body = Visit(node.DefaultBody);
            var switchValue = Visit(node.SwitchValue);
            if (body != node.DefaultBody ||
                switchValue != node.SwitchValue ||
                !switchCases.SequenceEqual(cases))
            {
                return Expression.Switch(switchValue, body, node.Comparison, switchCases);
            }

            return node;
        }

        protected virtual SwitchCase VisitSwitchCase(SwitchCase switchCase)
        {
            var body = Visit(switchCase.CheckNotNull(nameof(switchCase)).Body);
            var testValues = switchCase.TestValues.Select(Visit).ToList();
            if (body != switchCase.Body || !switchCase.TestValues.SequenceEqual(testValues))
            {
                return Expression.SwitchCase(body, testValues!);
            }

            return switchCase;
        }

        protected virtual Expression VisitTry(TryExpression node)
        {
            var body = Visit(node.CheckNotNull(nameof(node)).Body);
            var fault = Visit(node.Fault);
            var @finally = Visit(node.Finally);
            var handlers = node.Handlers.Select(VisitCatch).ToList();
            if (body != node.Body ||
                fault != node.Fault ||
                @finally != node.Finally ||
                !handlers.SequenceEqual(node.Handlers))
            {
                return Expression.MakeTry(node.Type, body, @finally, fault, handlers);
            }

            return node;
        }

        protected virtual CatchBlock VisitCatch(CatchBlock catchBlock)
        {
            var body = Visit(catchBlock.CheckNotNull(nameof(catchBlock)).Body);
            var filter = Visit(catchBlock.Filter);
            var variable = catchBlock.Variable is null ? null : (ParameterExpression)VisitParameter(catchBlock.Variable);
            if (body != catchBlock.Body || filter != catchBlock.Filter || variable != catchBlock.Variable)
            {
                return Expression.MakeCatchBlock(catchBlock.Test, variable, body, filter);
            }

            return catchBlock;
        }

        protected virtual MemberBinding VisitMemberBinding(MemberBinding binding)
            => binding.CheckNotNull(nameof(binding)).BindingType switch
            {
                MemberBindingType.Assignment => VisitMemberAssignment((MemberAssignment)binding),
                MemberBindingType.MemberBinding => VisitMemberMemberBinding((MemberMemberBinding)binding),
                MemberBindingType.ListBinding => VisitMemberListBinding((MemberListBinding)binding),
                _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
            };

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.CheckNotNull(nameof(initializer)).Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }

            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.CheckNotNull(nameof(node)).Operand);
            if (operand != node.Operand)
            {
                return Expression.MakeUnary(node.NodeType, operand, node.Type, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.CheckNotNull(nameof(node)).Left);
            var right = Visit(node.Right);
            var conversion = Visit(node.Conversion);
            if (left != node.Left || right != node.Right || conversion != node.Conversion)
            {
                return node.NodeType == ExpressionType.Coalesce && node.Conversion is not null
                    ? Expression.Coalesce(left, right, conversion as LambdaExpression)
                    : Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            var expr = Visit(node.CheckNotNull(nameof(node)).Expression);
            if (expr != node.Expression)
            {
                return Expression.TypeIs(expr, node.TypeOperand);
            }

            return node;
        }

        protected virtual Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        protected virtual Expression VisitConditional(ConditionalExpression node)
        {
            var test = Visit(node.CheckNotNull(nameof(node)).Test);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);

            if (test != node.Test || ifTrue != node.IfTrue || ifFalse != node.IfFalse)
            {
                return ifFalse is DefaultExpression
                    ? Expression.IfThen(test, ifTrue)
                    : Expression.Condition(test, ifTrue, ifFalse);
            }

            return node;
        }

        protected virtual Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            var exp = Visit(node.CheckNotNull(nameof(node)).Expression);
            if (exp != node.Expression)
            {
                return Expression.MakeMemberAccess(exp, node.Member);
            }

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var obj = Visit(node.CheckNotNull(nameof(node)).Object);
            var args = VisitExpressionList(node.Arguments);
            if (obj != node.Object || args != node.Arguments)
            {
                return Expression.Call(obj, node.Method, args);
            }

            return node;
        }

        protected virtual ReadOnlyCollection<T> VisitExpressionList<T>(ReadOnlyCollection<T> list)
            where T : Expression
        {
            list.AssertNotNull(nameof(list));
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

            return visited?.AsReadOnly() ?? list;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.CheckNotNull(nameof(assignment)).Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.CheckNotNull(nameof(binding)).Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.CheckNotNull(nameof(binding)).Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }

            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> list)
        {
            list.AssertNotNull(nameof(list));
            List<MemberBinding>? visited = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                var b = VisitMemberBinding(list[i]);
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

            return visited?.AsEnumerable() ?? list;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> list)
        {
            list.AssertNotNull(nameof(list));
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

            return visited?.AsEnumerable() ?? list;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            var body = Visit(node.CheckNotNull(nameof(node)).Body);
            if (body != node.Body)
            {
                return Expression.Lambda(node.Type, body, node.Parameters);
            }

            return node;
        }

        protected virtual Expression VisitNew(NewExpression node)
        {
            var args = VisitExpressionList(node.CheckNotNull(nameof(node)).Arguments);
            if (args != node.Arguments)
            {
                if (node.Constructor is null)
                {
                    return Expression.New(node.Type);
                }

                if (node.Members is null)
                {
                    return Expression.New(node.Constructor, args);
                }

                return Expression.New(node.Constructor, args, node.Members);
            }

            return node;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            var n = VisitNew(node.CheckNotNull(nameof(node)).NewExpression);
            var bindings = VisitBindingList(node.Bindings);
            if (n is NewExpression newExpression && (newExpression != node.NewExpression || bindings != node.Bindings))
            {
                return Expression.MemberInit(newExpression, bindings);
            }

            return node;
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            var n = VisitNew(node.CheckNotNull(nameof(node)).NewExpression);
            var initializers = VisitElementInitializerList(node.Initializers);
            if (n is NewExpression newExpression && (newExpression != node.NewExpression || initializers != node.Initializers))
            {
                return Expression.ListInit(newExpression, initializers);
            }

            return node;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            var exprs = VisitExpressionList(node.CheckNotNull(nameof(node)).Expressions);
            if (exprs != node.Expressions)
            {
                var elementType = node.Type.GetElementType() !;
                return node.NodeType == ExpressionType.NewArrayInit
                    ? Expression.NewArrayInit(elementType, exprs)
                    : Expression.NewArrayBounds(elementType, exprs);
            }

            return node;
        }

        protected virtual Expression VisitInvocation(InvocationExpression node)
        {
            var args = VisitExpressionList(node.CheckNotNull(nameof(node)).Arguments);
            var expr = Visit(node.Expression);
            if (args != node.Arguments || expr != node.Expression)
            {
                return Expression.Invoke(expr, args);
            }

            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node)
        {
            var expressions = VisitExpressionList(node.CheckNotNull(nameof(node)).Expressions);
            var variables = VisitExpressionList(node.Variables);
            if (expressions != node.Expressions || variables != node.Variables)
            {
                return Expression.Block(node.Type, variables, expressions);
            }

            return node;
        }

        protected virtual Expression VisitDefault(DefaultExpression node)
            => node;

        protected virtual Expression VisitGoto(GotoExpression node)
            => node;

        protected virtual Expression VisitLabel(LabelExpression node)
        {
            var defaultValue = Visit(node.CheckNotNull(nameof(node)).DefaultValue);
            if (!ReferenceEquals(defaultValue, node.DefaultValue))
            {
                return Expression.Label(node.Target, defaultValue);
            }

            return node;
        }

        protected virtual Expression VisitLoop(LoopExpression node)
        {
            var body = Visit(node.CheckNotNull(nameof(node)).Body);
            if (!ReferenceEquals(body, node.Body))
            {
                return Expression.Loop(body, node.BreakLabel, node.ContinueLabel);
            }

            return node;
        }
    }
}