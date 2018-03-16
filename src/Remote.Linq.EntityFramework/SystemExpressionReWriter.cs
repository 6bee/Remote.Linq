// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class SystemExpressionReWriter
    {
        /// <summary>
        /// Replaces parameterized constructor calls for <see cref="VariableQueryArgument{T}"/> with type initializer
        /// </summary>
        internal static Expression ReplaceParameterizedConstructorCallsForVariableQueryArguments(this Expression expression)
            => new ReWriter().Run(expression);

        private sealed class ReWriter : ExpressionVisitorBase
        {
            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression Visit(Expression expression)
            {
                if (expression?.NodeType == ExpressionType.New)
                {
                    return Visit((NewExpression)expression);
                }

                return base.Visit(expression);
            }

            private Expression Visit(NewExpression node)
            {
                var type = node.Type;
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>) &&
                    node.Constructor.GetParameters().Length == 1 &&
                    node.Arguments.Count == 1)
                {
                    var argument = node.Arguments.Single();
                    return Expression.MemberInit(
                        Expression.New(type),
                        Expression.Bind(type.GetProperty(nameof(VariableQueryArgument<object>.Value)), argument));
                }

                if (type == typeof(VariableQueryArgument) &&
                    node.Constructor.GetParameters().Length == 2 &&
                    node.Arguments.Any())
                {
                    var valueArgument = node.Arguments.First();

                    // Note: optional second type argument is omitted since it would not be supported by EF anyway
                    return Expression.MemberInit(
                        Expression.New(type),
                        Expression.Bind(type.GetProperty(nameof(VariableQueryArgument.Value)), valueArgument));
                }

                return VisitNew(node);
            }
        }
    }
}
