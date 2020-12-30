// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class RemoteExpressionReWriter
    {
        private static readonly MethodInfo QueryableIncludeMethod = typeof(System.Data.Entity.QueryableExtensions)
            .GetMethods()
            .Where(x => string.Equals(x.Name, nameof(System.Data.Entity.QueryableExtensions.Include), StringComparison.Ordinal))
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Single();

        /// <summary>
        /// Replaces resource descriptors by queryable and replaces include method call with entity framework's include methods.
        /// </summary>
        internal static Expression ReplaceIncludeMethodCall(this Expression expression)
            => new ElementReplacer().Run(expression);

        private sealed class ElementReplacer : RemoteExpressionVisitorBase
        {
            internal Expression Run(Expression expression)
                => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression expression)
            {
                if (expression.Instance is null &&
                    expression.Method.DeclaringType.Type == typeof(QueryFunctions) &&
                    string.Equals(expression.Method.Name, nameof(QueryFunctions.Include), StringComparison.Ordinal) &&
                    expression.Method.GenericArgumentTypes.Count == 1 &&
                    expression.Arguments.Count == 2)
                {
                    var elementType = expression.Method.GenericArgumentTypes.Single().Type;

                    var efIncludeMethod = QueryableIncludeMethod.MakeGenericMethod(elementType);

                    var callExpression = new MethodCallExpression(null, efIncludeMethod, expression.Arguments);
                    expression = callExpression;
                }

                return base.VisitMethodCall(expression);
            }
        }
    }
}
