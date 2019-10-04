// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
        protected virtual Expression Visit(Expression expression)
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
                    throw new Exception($"Unhandled expression type: '{expression.NodeType}'");
            }
        }

        protected virtual Expression VisitSwitch(SwitchExpression switchExpression)
        {
            List<SwitchCase> cases = (switchExpression.Cases ?? Enumerable.Empty<SwitchCase>()).ToList();
            List<SwitchCase> switchCases = cases.Select(VisitSwitchCase).ToList();
            Expression body = Visit(switchExpression.DefaultBody);
            Expression switchValue = Visit(switchExpression.SwitchValue);
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
            Expression body = Visit(switchCase.Body);
            List<Expression> testValues = switchCase.TestValues.Select(Visit).ToList();
            if (body != switchCase.Body || switchCase.TestValues.SequenceEqual(testValues) == false)
            {
                return Expression.SwitchCase(body, testValues);
            }

            return switchCase;
        }

        protected virtual Expression VisitTry(TryExpression tryExpression)
        {
            Expression body = Visit(tryExpression.Body);
            Expression fault = Visit(tryExpression.Fault);
            Expression @finally = Visit(tryExpression.Finally);
            List<CatchBlock> handlers = tryExpression.Handlers.Select(VisitCatch).ToList();
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
            Expression body = Visit(catchBlock.Body);
            Expression filter = Visit(catchBlock.Filter);
            ParameterExpression variable = (ParameterExpression)VisitParameter(catchBlock.Variable);
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
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }

            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            Expression operand = Visit(node.Operand);
            if (operand != node.Operand)
            {
                return Expression.MakeUnary(node.NodeType, operand, node.Type, node.Method);
            }

            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            Expression left = Visit(node.Left);
            Expression right = Visit(node.Right);
            Expression conversion = Visit(node.Conversion);
            if (left != node.Left || right != node.Right || conversion != node.Conversion)
            {
                if (node.NodeType == ExpressionType.Coalesce && node.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                else
                {
                    return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
                }
            }

            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            Expression expr = Visit(node.Expression);
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
            Expression test = Visit(node.Test);
            Expression ifTrue = Visit(node.IfTrue);
            Expression ifFalse = Visit(node.IfFalse);

            if (test != node.Test || ifTrue != node.IfTrue || ifFalse != node.IfFalse)
            {
                if (ifFalse is DefaultExpression)
                {
                    return Expression.IfThen(test, ifTrue);
                }
                else
                {
                    return Expression.Condition(test, ifTrue, ifFalse);
                }
            }

            return node;
        }

        protected virtual Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            Expression exp = Visit(node.Expression);
            if (exp != node.Expression)
            {
                return Expression.MakeMemberAccess(exp, node.Member);
            }

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression obj = Visit(node.Object);
            IEnumerable<Expression> args = VisitExpressionList(node.Arguments);
            if (obj != node.Object || args != node.Arguments)
            {
                return Expression.Call(obj, node.Method, args);
            }

            return node;
        }

        protected virtual ReadOnlyCollection<T> VisitExpressionList<T>(ReadOnlyCollection<T> original) where T : Expression
        {
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
                return list.AsReadOnly();
            }

            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }

            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = VisitMemberBinding(original[i]);
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

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
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

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            Expression body = Visit(node.Body);
            if (body != node.Body)
            {
                return Expression.Lambda(node.Type, body, node.Parameters);
            }

            return node;
        }

        protected virtual NewExpression VisitNew(NewExpression node)
        {
            IEnumerable<Expression> args = VisitExpressionList(node.Arguments);
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
            NewExpression n = VisitNew(node.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(node.Bindings);
            if (n != node.NewExpression || bindings != node.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }

            return node;
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            NewExpression n = VisitNew(node.NewExpression);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(node.Initializers);
            if (n != node.NewExpression || initializers != node.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }

            return node;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(node.Expressions);
            if (exprs != node.Expressions)
            {
                if (node.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(node.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(node.Type.GetElementType(), exprs);
                }
            }

            return node;
        }

        protected virtual Expression VisitInvocation(InvocationExpression node)
        {
            IEnumerable<Expression> args = VisitExpressionList(node.Arguments);
            Expression expr = Visit(node.Expression);
            if (args != node.Arguments || expr != node.Expression)
            {
                return Expression.Invoke(expr, args);
            }

            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node)
        {
            IEnumerable<Expression> expressions = VisitExpressionList(node.Expressions);
            IEnumerable<ParameterExpression> variables = VisitExpressionList(node.Variables);
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
