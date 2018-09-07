// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class RemoteExpressionVisitorBase
    {
        protected virtual Expression Visit(Expression expression)
        {
            if (expression is null)
            {
                return null;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Binary:
                    return VisitBinary((BinaryExpression)expression);

                case ExpressionType.Block:
                    return VisitBlock((BlockExpression)expression);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)expression);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)expression);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)expression);

                case ExpressionType.Default:
                    return VisitDefault((DefaultExpression)expression);

                case ExpressionType.Goto:
                    return VisitGoto((GotoExpression)expression);

                case ExpressionType.Invoke:
                    return VisitInvoke((InvokeExpression)expression);

                case ExpressionType.Label:
                    return VisitLabel((LabelExpression)expression);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expression);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)expression);

                case ExpressionType.Loop:
                    return VisitLoop((LoopExpression)expression);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)expression);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)expression);

                case ExpressionType.New:
                    return VisitNew((NewExpression)expression);

                case ExpressionType.NewArray:
                    return VisitNewArray((NewArrayExpression)expression);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)expression);

                case ExpressionType.Switch:
                    return VisitSwitch((SwitchExpression)expression);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)expression);

                case ExpressionType.Try:
                    return VisitTry((TryExpression)expression);

                case ExpressionType.Unary:
                    return VisitUnary((UnaryExpression)expression);

                default:
                    throw new Exception(string.Format("Unknown expression type: '{0}'", expression.NodeType));
            }
        }

        protected virtual Expression VisitSwitch(SwitchExpression switchExpression)
        {
            Expression defaultExpression = Visit(switchExpression.DefaultExpression);
            Expression switchValue = Visit(switchExpression.SwitchValue);
            List<SwitchCase> cases = (switchExpression.Cases ?? Enumerable.Empty<SwitchCase>()).Select(VisitSwitchCase).ToList();
            if (defaultExpression != switchExpression.DefaultExpression ||
                switchValue != switchExpression.SwitchValue ||
                cases.SequenceEqual(switchExpression.Cases) == false)
            {
                return new SwitchExpression(switchValue, switchExpression.Comparison, defaultExpression, cases);
            }

            return switchExpression;
        }

        protected virtual SwitchCase VisitSwitchCase(SwitchCase switchCase)
        {
            Expression body = Visit(switchCase.Body);
            List<Expression> testValues = switchCase.TestValues ?? new List<Expression>();
            List<Expression> newTestValues = testValues.Select(Visit).ToList();
            if (body != switchCase.Body || testValues.SequenceEqual(newTestValues) == false)
            {
                return new SwitchCase(body, newTestValues);
            }

            return switchCase;
        }

        protected virtual Expression VisitTry(TryExpression tryExpression)
        {
            Expression @finally = Visit(tryExpression.Finally);
            Expression body = Visit(tryExpression.Body);
            Expression fault = Visit(tryExpression.Fault);
            List<CatchBlock> handlers = (tryExpression.Handlers ?? Enumerable.Empty<CatchBlock>()).Select(VisitCatchBlock).ToList();
            if (@finally != tryExpression.Finally ||
                body != tryExpression.Body ||
                fault != tryExpression.Fault ||
                handlers.SequenceEqual(tryExpression.Handlers) == false)
            {
                return new TryExpression(tryExpression.Type, body, fault, @finally, handlers);
            }

            return tryExpression;
        }

        protected virtual CatchBlock VisitCatchBlock(CatchBlock catchBlock)
        {
            Expression body = Visit(catchBlock.Body);
            ParameterExpression variable = VisitParameter(catchBlock.Variable);
            Expression filter = Visit(catchBlock.Filter);
            if (body != catchBlock.Body || variable != catchBlock.Variable || filter != catchBlock.Filter)
            {
                return new CatchBlock(catchBlock.Test, variable, body, filter);
            }

            return catchBlock;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            NewExpression n = VisitNew(node.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(node.Bindings);

            if (n != node.NewExpression || bindings != node.Bindings)
            {
                return new MemberInitExpression(n, bindings);
            }

            return node;
        }

        protected virtual List<MemberBinding> VisitBindingList(List<MemberBinding> original)
        {
            if (original is null)
            {
                return null;
            }

            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);

                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = Visit(assignment.Expression);

            if (e != assignment.Expression)
            {
                return new MemberAssignment(assignment.Member, e);
            }

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings.ToList());

            if (bindings != binding.Bindings)
            {
                return new MemberMemberBinding(binding.Member, bindings);
            }

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);

            if (!ReferenceEquals(initializers, binding.Initializers))
            {
                return new MemberListBinding(binding.Member, initializers);
            }

            return binding;
        }

        protected virtual List<ElementInit> VisitElementInitializerList(List<ElementInit> original)
        {
            if (original is null)
            {
                return null;
            }

            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            IEnumerable<Expression> arguments = VisitExpressionList(initializer.Arguments);

            if (!ReferenceEquals(arguments, initializer.Arguments))
            {
                return new ElementInit(initializer.AddMethod.Method, arguments);
            }

            return initializer;
        }

        protected virtual List<T> VisitExpressionList<T>(List<T> original) where T : Expression
        {
            if (original is null)
            {
                return null;
            }

            List<T> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                T p = (T)Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<T>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            if (list != null)
            {
                return list;
            }

            return original;
        }

        protected virtual NewExpression VisitNew(NewExpression node)
        {
            var args = VisitExpressionList(node.Arguments);

            if (!ReferenceEquals(args, node.Arguments))
            {
                return node.Constructor is null
                    ? new NewExpression(node.Type)
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
            var newExpression = VisitNew(node.NewExpression);

            if (!ReferenceEquals(newExpression, node.NewExpression))
            {
                return new ListInitExpression(newExpression, node.Initializers);
            }

            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            var leftOperand = Visit(node.LeftOperand);
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
            var expression = Visit(node.Expression);
            var arguments = VisitExpressionList(node.Arguments);

            if (!ReferenceEquals(expression, node.Expression) || !ReferenceEquals(arguments, node.Arguments))
            {
                return new InvokeExpression(expression, arguments);
            }

            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node)
        {
            var variables = VisitExpressionList(node.Variables);
            var expressions = VisitExpressionList(node.Expressions);

            if (!ReferenceEquals(variables, node.Variables) || !ReferenceEquals(expressions, node.Expressions))
            {
                return new BlockExpression(node.Type, variables, expressions);
            }

            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            var exp = Visit(node.Expression);

            if (!ReferenceEquals(exp, node.Expression))
            {
                return new TypeBinaryExpression(exp, node.TypeOperand);
            }

            return node;
        }

        protected virtual Expression VisitConditional(ConditionalExpression node)
        {
            var test = Visit(node.Test);
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
            var instance = Visit(node.Expression);

            if (!ReferenceEquals(instance, node.Expression))
            {
                return new MemberExpression(instance, node.Member);
            }

            return node;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);

            if (!ReferenceEquals(operand, node.Operand))
            {
                return new UnaryExpression(node.UnaryOperator, operand, node.Type, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var instance = Visit(node.Instance);
            var argumements =
                from i in node.Arguments
                select new { Old = i, New = Visit(i) };

            argumements = argumements.ToList();

            if (!ReferenceEquals(instance, node.Instance) || argumements.Any(i => !ReferenceEquals(i.Old, i.New)))
            {
                return new MethodCallExpression(instance, node.Method, argumements.Select(i => i.New));
            }

            return node;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            var exp = Visit(node.Expression);
            var parameters =
                from i in node.Parameters
                select new { Old = i, New = VisitParameter(i) };

            parameters = parameters.ToList();

            if (!ReferenceEquals(exp, node.Expression) || parameters.Any(i => !ReferenceEquals(i.Old, i.New)))
            {
                return new LambdaExpression(node.Type, exp, parameters.Select(i => i.New));
            }

            return node;
        }

        protected virtual Expression VisitDefault(DefaultExpression node)
            => node;

        protected virtual Expression VisitGoto(GotoExpression node)
        {
            var value = Visit(node.Value);

            if (!ReferenceEquals(value, node.Value))
            {
                return new GotoExpression(node.Kind, node.Target, node.Type, node.Value);
            }

            return node;
        }

        protected virtual Expression VisitLabel(LabelExpression node)
        {
            var defaultValue = Visit(node.DefaultValue);

            if (!ReferenceEquals(defaultValue, node.DefaultValue))
            {
                return new LabelExpression(node.Target, defaultValue);
            }

            return node;
        }

        protected virtual LoopExpression VisitLoop(LoopExpression node)
        {
            var body = Visit(node.Body);

            if (!ReferenceEquals(body, node.Body))
            {
                return new LoopExpression(body, node.BreakLabel, node.ContinueLabel);
            }

            return node;
        }
    }
}
