// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Extensions.Include
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryableExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> queryable, string navigationPropertyPath)
        {
            queryable.CheckNotNull(nameof(queryable));
            navigationPropertyPath.CheckNotNull(nameof(navigationPropertyPath));
            if (string.IsNullOrEmpty(navigationPropertyPath))
            {
                throw new ArgumentException("Navigation property path must not be empty", nameof(navigationPropertyPath));
            }

            if (queryable is IRemoteQueryable<T>)
            {
                var methodInfo = MethodInfos.IncludeQueryFunctions.Include.MakeGenericMethod(typeof(T));
                var parameter1 = queryable.Expression;
                var parameter2 = Expression.Constant(navigationPropertyPath);
                var methodCallExpression = Expression.Call(methodInfo, parameter1, parameter2);
                var newQueryable = queryable.Provider.CreateQuery<T>(methodCallExpression);
                return newQueryable;
            }

            return queryable;
        }

        public static IIncludableRemoteQueryable<T, TProperty> Include<T, TProperty>(this IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            queryable.CheckNotNull(nameof(queryable));

            if (queryable is IRemoteQueryable<T> remoteQueryable)
            {
                return IncludeCore(remoteQueryable, navigationPropertyPath, false);
            }

            throw new ArgumentException($"{nameof(Include)} is supported for IRemoteQueryable<> only.", nameof(queryable));
        }

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, TPreviousProperty> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath, true);

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, IEnumerable<TPreviousProperty>> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath, true);

        private static IIncludableRemoteQueryable<T, TNavigationProperty> IncludeCore<T, TNavigationSource, TNavigationProperty>(IRemoteQueryable<T> queryable, Expression<Func<TNavigationSource, TNavigationProperty>> navigationPropertyPath, bool isNestedStatement)
        {
            if (queryable is null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if (navigationPropertyPath is null)
            {
                throw new ArgumentNullException(nameof(navigationPropertyPath));
            }

            if (!TryParsePath(navigationPropertyPath.Body, out var path) || path is null)
            {
                throw new ArgumentException("Invalid include path expression", nameof(navigationPropertyPath));
            }

            var queryableBase = queryable;
            if (isNestedStatement && queryable is IStackedIncludableQueryable<T> preceding)
            {
                queryableBase = preceding.Parent;
                path = $"{preceding.IncludePath}.{path}";
            }

            var includeExpresson = queryableBase.Include(path);
            return new IncludableRemoteQueryable<T, TNavigationProperty>(queryable.Provider, includeExpresson.Expression, queryableBase, path);
        }

        private static bool TryParsePath(Expression expression, out string? path)
        {
            path = null;
            var expression1 = RemoveConvert(expression);
            if (expression1 is MemberExpression memberExpression)
            {
                if (memberExpression.Expression is null)
                {
                    return false;
                }

                var name = memberExpression.Member.Name;
                if (!TryParsePath(memberExpression.Expression, out var path1))
                {
                    return false;
                }

                path = path1 is null ? name : $"{path1}.{name}";
            }
            else if (expression1 is MethodCallExpression methodCallExpression)
            {
                if (string.Equals(methodCallExpression.Method.Name, "Select", StringComparison.Ordinal) &&
                    methodCallExpression.Arguments.Count == 2 &&
                    TryParsePath(methodCallExpression.Arguments[0], out var path1) &&
                    path1 is not null &&
                    methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression &&
                    TryParsePath(lambdaExpression.Body, out var path2) &&
                    path2 is not null)
                {
                    path = $"{path1}.{path2}";
                    return true;
                }

                return false;
            }

            return true;
        }

        private static Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }
    }
}