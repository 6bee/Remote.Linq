// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionVisitors
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SystemExpressionReWriter
    {
        /// <summary>
        /// Replaces parameterized constructor calls for <see cref="VariableQueryArgument{T}"/> with type initializer.
        /// </summary>
        public static Expression ReplaceParameterizedConstructorCallsForVariableQueryArguments(this Expression expression)
            => new VariableQueryArgumentsReWriter().Run(expression);

        private sealed class VariableQueryArgumentsReWriter : ExpressionVisitorBase
        {
            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitNew(NewExpression node)
            {
                var type = node.Type;

                if (type == typeof(VariableQueryArgument) &&
                    node.Constructor?.GetParameters().Length == 2 &&
                    node.Arguments?.Any() is true)
                {
                    var valueArgument = node.Arguments.Select(Visit).First();

                    // Note: second parameter (i.e. optional type argument) is omitted since not supported by EF anyway
                    return Expression.MemberInit(
                        Expression.New(type),
                        Expression.Bind(type.GetProperty(nameof(VariableQueryArgument.Value)), valueArgument));
                }

                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>) &&
                    node.Constructor?.GetParameters().Length == 1 &&
                    node.Arguments?.Count == 1)
                {
                    var argument = node.Arguments.Select(Visit).Single();
                    return Expression.MemberInit(
                        Expression.New(type),
                        Expression.Bind(type.GetProperty(nameof(VariableQueryArgument<object>.Value)), argument));
                }

                return base.VisitNew(node);
            }
        }
    }
}