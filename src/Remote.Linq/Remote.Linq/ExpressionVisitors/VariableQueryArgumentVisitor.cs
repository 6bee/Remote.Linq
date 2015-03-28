// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;

    public class VariableQueryArgumentVisitor
    {
        internal Expression ReplaceNonGenericQueryArgumentsByGenericArguments(Expression expression)
        {
            return new NonGenericVariableQueryArgumentVisitor().ReplaceNonGenericQueryArgumentsByGenericArguments(expression);
        }

        internal Expression ReplaceGenericQueryArgumentsByNonGenericArguments(Expression expression)
        {
            return new GenericVariableQueryArgumentVisitor().ReplaceGenericQueryArgumentsByNonGenericArguments(expression);
        }

        protected class GenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly PropertyInfo QueryArgumentValuePropertyInfo = new PropertyInfo(typeof(VariableQueryArgument).GetProperty("Value"));

            public Expression ReplaceGenericQueryArgumentsByNonGenericArguments(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var type = expression.Type;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                {
                    var valueProperty = expression.Value.GetType().GetProperty("Value");
                    var value = valueProperty.GetValue(expression.Value);

                    var queryArgument = new VariableQueryArgument(value, valueProperty.PropertyType);
                    return Expression.Constant(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                if (member.MemberType == MemberTypes.Property && member.DeclaringType.IsGenericType && member.DeclaringType.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                {
                    var instanceExpression = Visit(expression.Expression);
                    var newMemberExpression = new MemberExpression(instanceExpression, QueryArgumentValuePropertyInfo);
                    return newMemberExpression;
                }

                return base.VisitMemberAccess(expression);
            }
        }

        protected class NonGenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            public Expression ReplaceNonGenericQueryArgumentsByGenericArguments(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var nonGenericQueryArgument = expression.Value as VariableQueryArgument;
                if (!ReferenceEquals(null, nonGenericQueryArgument))
                {
                    var type = nonGenericQueryArgument.Type.Type;
                    var value = nonGenericQueryArgument.Value;
                    var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(type), new[] { value });
                    return Expression.Constant(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                if (member.MemberType == MemberTypes.Property && member.DeclaringType.FullName == typeof(VariableQueryArgument).FullName)
                {
                    var instanceExpression = Visit(expression.Expression) as ConstantExpression;
                    if (!ReferenceEquals(null, instanceExpression))
                    {
                        var instanceType = instanceExpression.Type;
                        if (instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                        {
                            var valuePropertyInfo = new PropertyInfo("Value", instanceType);

                            var newMemberExpression = new MemberExpression(instanceExpression, valuePropertyInfo);
                            return newMemberExpression;
                        }
                    }
                }

                return base.VisitMemberAccess(expression);
            }
        }
    }
}
