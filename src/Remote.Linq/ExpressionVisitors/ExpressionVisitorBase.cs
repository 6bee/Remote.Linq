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
    public abstract class ExpressionVisitorBase
    {
        [return: NotNullIfNotNull("expression")]
        protected virtual Expression? Visit(Expression? expression)
        {
            if (expression is null)
            {
                return null;
            }

            if (expression is UnaryExpression unaryExpression)
            {
                return VisitUnary(unaryExpression);
            }

            if (expression is BinaryExpression binaryExpression)
            {
                return VisitBinary(binaryExpression);
            }

            switch (expression.NodeType)
            {
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
                    return VisitInvocation((InvocationExpression)expression);

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

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)expression);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)expression);

                case ExpressionType.Switch:
                    return VisitSwitch((SwitchExpression)expression);

                case ExpressionType.Try:
                    return VisitTry((TryExpression)expression);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)expression);

                default:
                    throw new RemoteLinqException($"Unhandled expression type: '{expression.NodeType}'");
            }
        }

        protected virtual Expression VisitSwitch(SwitchExpression switchExpression)
        {
            var cases = (switchExpression.Cases ?? Enumerable.Empty<SwitchCase>()).ToList();
            var switchCases = cases.Select(VisitSwitchCase).ToList();
            var body = Visit(switchExpression.DefaultBody);
            var switchValue = Visit(switchExpression.SwitchValue);
            if (body != switchExpression.DefaultBody ||
                switchValue != switchExpression.SwitchValue ||
                switchCases.SequenceEqual(cases) == false)
            {
                return Expression.Switch(switchValue, body, switchExpression.Comparison, switchCases);
            }

            return switchExpression;
        }

        protected virtual SwitchCase VisitSwitchCase(SwitchCase switchCase)
        {
            var body = Visit(switchCase.Body);
            var testValues = switchCase.TestValues.Select(Visit).ToList();
            if (body != switchCase.Body || switchCase.TestValues.SequenceEqual(testValues) == false)
            {
                return Expression.SwitchCase(body, testValues);
            }

            return switchCase;
        }

        protected virtual Expression VisitTry(TryExpression tryExpression)
        {
            var body = Visit(tryExpression.Body);
            var fault = Visit(tryExpression.Fault);
            var @finally = Visit(tryExpression.Finally);
            var handlers = tryExpression.Handlers.Select(VisitCatch).ToList();
            if (body != tryExpression.Body ||
                fault != tryExpression.Fault ||
                @finally != tryExpression.Finally ||
                handlers.SequenceEqual(tryExpression.Handlers) == false)
            {
                return Expression.MakeTry(tryExpression.Type, body, @finally, fault, handlers);
            }

            return tryExpression;
        }

        protected virtual CatchBlock VisitCatch(CatchBlock catchBlock)
        {
            var body = Visit(catchBlock.Body);
            var filter = Visit(catchBlock.Filter);
            var variable = (ParameterExpression)VisitParameter(catchBlock.Variable);
            if (body != catchBlock.Body || filter != catchBlock.Filter || variable != catchBlock.Variable)
            {
                return Expression.MakeCatchBlock(catchBlock.Test, variable, body, filter);
            }

            return catchBlock;
        }

        protected virtual MemberBinding VisitMemberBinding(MemberBinding binding)
            => binding.BindingType switch
            {
                MemberBindingType.Assignment => VisitMemberAssignment((MemberAssignment)binding),
                MemberBindingType.MemberBinding => VisitMemberMemberBinding((MemberMemberBinding)binding),
                MemberBindingType.ListBinding => VisitMemberListBinding((MemberListBinding)binding),
                _ => throw new Exception($"Unhandled binding type '{binding.BindingType}'"),
            };

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }

            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);
            if (operand != node.Operand)
            {
                return Expression.MakeUnary(node.NodeType, operand, node.Type, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            var conversion = Visit(node.Conversion);
            if (left != node.Left || right != node.Right || conversion != node.Conversion)
            {
                return node.NodeType == ExpressionType.Coalesce && node.Conversion != null
                    ? Expression.Coalesce(left, right, conversion as LambdaExpression)
                    : Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            var expr = Visit(node.Expression);
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
            var test = Visit(node.Test);
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
            var exp = Visit(node.Expression);
            if (exp != node.Expression)
            {
                return Expression.MakeMemberAccess(exp, node.Member);
            }

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var obj = Visit(node.Object);
            var args = VisitExpressionList(node.Arguments);
            if (obj != node.Object || args != node.Arguments)
            {
                return Expression.Call(obj, node.Method, args);
            }

            return node;
        }

        protected virtual ReadOnlyCollection<T> VisitExpressionList<T>(ReadOnlyCollection<T> original) where T : Expression
        {
            List<T>? list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = (T)Visit(original[i]);
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

            return list?.AsReadOnly() ?? original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }

            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding>? list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = VisitMemberBinding(original[i]);
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

            return list?.AsEnumerable() ?? original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit>? list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = VisitElementInitializer(original[i]);
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

            return list?.AsEnumerable() ?? original;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            var body = Visit(node.Body);
            if (body != node.Body)
            {
                return Expression.Lambda(node.Type, body, node.Parameters);
            }

            return node;
        }

        protected virtual NewExpression VisitNew(NewExpression node)
        {
            var args = VisitExpressionList(node.Arguments);
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
            var n = VisitNew(node.NewExpression);
            var bindings = VisitBindingList(node.Bindings);
            if (n != node.NewExpression || bindings != node.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }

            return node;
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            var n = VisitNew(node.NewExpression);
            var initializers = VisitElementInitializerList(node.Initializers);
            if (n != node.NewExpression || initializers != node.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }

            return node;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            var exprs = VisitExpressionList(node.Expressions);
            if (exprs != node.Expressions)
            {
                return node.NodeType == ExpressionType.NewArrayInit
                    ? Expression.NewArrayInit(node.Type.GetElementType(), exprs)
                    : Expression.NewArrayBounds(node.Type.GetElementType(), exprs);
            }

            return node;
        }

        protected virtual Expression VisitInvocation(InvocationExpression node)
        {
            var args = VisitExpressionList(node.Arguments);
            var expr = Visit(node.Expression);
            if (args != node.Arguments || expr != node.Expression)
            {
                return Expression.Invoke(expr, args);
            }

            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node)
        {
            var expressions = VisitExpressionList(node.Expressions);
            var variables = VisitExpressionList(node.Variables);
            if (expressions != node.Expressions || variables != node.Variables)
            {
                return Expression.Block(node.Type, variables, expressions);
            }

            return node;
        }

        protected virtual Expression VisitDefault(DefaultExpression node)
        {
            return node;
        }

        protected virtual Expression VisitGoto(GotoExpression node)
        {
            return node;
        }

        protected virtual Expression VisitLabel(LabelExpression node)
        {
            var defaultValue = Visit(node.DefaultValue);
            if (!ReferenceEquals(defaultValue, node.DefaultValue))
            {
                return Expression.Label(node.Target, defaultValue);
            }

            return node;
        }

        protected virtual Expression VisitLoop(LoopExpression node)
        {
            var body = Visit(node.Body);
            if (!ReferenceEquals(body, node.Body))
            {
                return Expression.Loop(body, node.BreakLabel, node.ContinueLabel);
            }

            return node;
        }
    }
}
