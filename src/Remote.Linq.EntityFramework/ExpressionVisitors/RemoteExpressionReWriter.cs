// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionVisitors
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class RemoteExpressionReWriter
    {
        private static readonly System.Reflection.MethodInfo QueryableIncludeMethod = typeof(System.Data.Entity.QueryableExtensions)
            .GetMethods()
            .Where(x => string.Equals(x.Name, nameof(System.Data.Entity.QueryableExtensions.Include), StringComparison.Ordinal))
            .Single(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1);

        /// <summary>
        /// Replaces resource descriptors by queryable and replaces include method call with entity framework's include methods.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be examined.</param>
        /// <returns>A new expression tree with the EF's version of the include method if any call the an include methods was contained in the original expression,
        /// otherwise the original expression tree is returned.</returns>
        internal static Expression ReplaceIncludeMethodCall(this Expression expression) => new ElementReplacer().Run(expression);

        private sealed class ElementReplacer : RemoteExpressionVisitorBase
        {
            internal Expression Run(Expression expression) => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression expression)
            {
                if (expression.Instance is null &&
                    string.Equals(expression.Method?.Name, nameof(QueryFunctions.Include), StringComparison.Ordinal) &&
                    expression.Method?.DeclaringType?.Type == typeof(QueryFunctions) &&
                    expression.Method?.GenericArgumentTypes?.Count == 1 &&
                    expression.Arguments?.Count == 2)
                {
                    var elementType = expression.Method.GenericArgumentTypes.Single().Type;

                    var queryableExpression = expression.Arguments[0];
                    var pathExpression = expression.Arguments[1];

                    var efIncludeMethod = QueryableIncludeMethod.MakeGenericMethod(elementType);

                    var callExpression = new MethodCallExpression(null, efIncludeMethod, new[] { queryableExpression, pathExpression });
                    expression = callExpression;
                }

                return base.VisitMethodCall(expression);
            }
        }
    }
}
