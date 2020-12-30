// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using MethodInfo = System.Reflection.MethodInfo;
    using RemoteLinq = Remote.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryableExtensions
    {
        private static readonly Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression> _defaultExpressionVisitor = RemoteExpressionReWriter.ReplaceNonGenericQueryArgumentsByGenericArguments;

        /// <summary>
        /// Execute the <see cref="IQueryable"/> and return the result without any extra tranformation.
        /// </summary>
        public static TResult Execute<TResult>(this IQueryable source)
        {
            source.CheckNotNull(nameof(source));
            return source.Provider.Execute<TResult>(source.Expression);
        }

        private static IOrderedQueryable<T> Sort<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression, MethodInfo methodInfo)
        {
            queryable.CheckNotNull(nameof(queryable));
            var exp = lambdaExpression.CheckNotNull(nameof(lambdaExpression)).Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), resultType);
            var lambdaExpressionMethodInfo = MethodInfos.Expression.Lambda.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = methodInfo.MakeGenericMethod(typeof(T), resultType);
            var result = method.Invoke(null, new object[] { queryable, funcExpression });

            return (IOrderedQueryable<T>)result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.OrderBy);

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.OrderByDescending);

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.ThenBy);

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.ThenByDescending);

        /// <summary>
        /// Applies this query instance to a queryable.
        /// </summary>
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, IQuery<T> query, Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression>? expressionVisitor)
        {
            var visitor = expressionVisitor ?? _defaultExpressionVisitor;
            return queryable
                .ApplyFilters(query, visitor)
                .ApplySorting(query, visitor)
                .ApplyPaging(query);
        }

        /// <summary>
        /// Applies this query instance to a queryable.
        /// </summary>
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, IQuery query, Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression>? expressionVisitor)
        {
            var q = query.ToGenericQuery<T>();
            return queryable.ApplyQuery(q, expressionVisitor ?? _defaultExpressionVisitor);
        }

        private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> queriable, IQuery<T> query, Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression> expressionVisitor)
        {
            queriable.CheckNotNull(nameof(queriable));
            query.CheckNotNull(nameof(query));
            foreach (var filter in query.FilterExpressions ?? Enumerable.Empty<RemoteLinq.LambdaExpression>())
            {
                var predicate = expressionVisitor(filter).ToLinqExpression<T, bool>();
                queriable = queriable.Where(predicate);
            }

            return queriable;
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> queriable, IQuery<T> query, Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression> expressionVisitor)
        {
            IOrderedQueryable<T>? orderedQueriable = null;
            foreach (var sort in query.SortExpressions ?? Enumerable.Empty<RemoteLinq.SortExpression>())
            {
                var exp = expressionVisitor(sort.Operand).ToLinqExpression();
                if (orderedQueriable is null)
                {
                    orderedQueriable = sort.SortDirection switch
                    {
                        RemoteLinq.SortDirection.Ascending => queriable.OrderBy(exp),
                        RemoteLinq.SortDirection.Descending => queriable.OrderByDescending(exp),
                        _ => throw new RemoteLinqException("not reachable code"),
                    };
                }
                else
                {
                    orderedQueriable = sort.SortDirection switch
                    {
                        RemoteLinq.SortDirection.Ascending => orderedQueriable.ThenBy(exp),
                        RemoteLinq.SortDirection.Descending => orderedQueriable.ThenByDescending(exp),
                        _ => throw new RemoteLinqException("not reachable code"),
                    };
                }
            }

            return orderedQueriable ?? queriable;
        }

        private static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queriable, IQuery<T> query)
        {
            if (query.SkipValue.HasValue)
            {
                queriable = queriable.Skip(query.SkipValue.Value);
            }

            if (query.TakeValue.HasValue)
            {
                queriable = queriable.Take(query.TakeValue.Value);
            }

            return queriable;
        }

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
                var methodInfo = MethodInfos.QueryFuntion.Include.MakeGenericMethod(typeof(T));
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
                return IncludeCore(remoteQueryable, navigationPropertyPath);
            }

            throw new ArgumentException($"{nameof(Include)} is supported for IRemoteQueryable<> only.", nameof(queryable));
        }

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, TPreviousProperty> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath);

        public static IIncludableRemoteQueryable<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(this IIncludableRemoteQueryable<T, IEnumerable<TPreviousProperty>> queryable, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            => IncludeCore(queryable, navigationPropertyPath);

        private static IIncludableRemoteQueryable<T, TNavigationProperty> IncludeCore<T, TNavigationSource, TNavigationProperty>(IRemoteQueryable<T> queryable, Expression<Func<TNavigationSource, TNavigationProperty>> navigationPropertyPath)
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
            if (queryable is IStackedIncludableQueryable<T> preceding)
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

        internal static Type? AsQueryableResourceTypeOrNull(this object? value)
        {
            if (value is IRemoteResource remoteResource)
            {
                return remoteResource.ResourceType;
            }

            if (value is IQueryable queryable)
            {
                var type = queryable.GetType();
                if (type.IsGenericType && typeof(EnumerableQuery<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    return null;
                }

                return queryable.ElementType;
            }

            return null;
        }
    }
}
