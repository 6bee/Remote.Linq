// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.SimpleQuery
{
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using MethodInfo = System.Reflection.MethodInfo;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SimpleQueryQueryableExtensions
    {
        private static readonly Func<RemoteLinq.LambdaExpression, RemoteLinq.LambdaExpression> _defaultExpressionVisitor = RemoteExpressionReWriter.ReplaceNonGenericQueryArgumentsByGenericArguments;

        /// <summary>
        /// Execute the <see cref="IQueryable"/> and return the result without any extra tranformation.
        /// </summary>
        public static TResult Execute<TResult>(this IQueryable source)
        {
            source.AssertNotNull(nameof(source));
            return source.Provider.Execute<TResult>(source.Expression);
        }

        private static IOrderedQueryable<T> Sort<T>(this IQueryable<T> queryable, SystemLinq.LambdaExpression lambdaExpression, MethodInfo methodInfo)
        {
            queryable.AssertNotNull(nameof(queryable));
            var exp = lambdaExpression.CheckNotNull(nameof(lambdaExpression)).Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), resultType);
            var lambdaExpressionMethodInfo = MethodInfos.Expression.Lambda.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = methodInfo.MakeGenericMethod(typeof(T), resultType);
            var result = method.Invoke(null, new object[] { queryable, funcExpression! });

            return (IOrderedQueryable<T>)result!;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, SystemLinq.LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.OrderBy);

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, SystemLinq.LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.OrderByDescending);

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, SystemLinq.LambdaExpression lambdaExpression)
            => queryable.Sort(lambdaExpression, MethodInfos.Queryable.ThenBy);

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, SystemLinq.LambdaExpression lambdaExpression)
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
            queriable.AssertNotNull(nameof(queriable));
            query.AssertNotNull(nameof(query));
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
                        _ => throw new InvalidOperationException($"Invalid {nameof(RemoteLinq.SortDirection)} '{sort.SortDirection}'"),
                    };
                }
                else
                {
                    orderedQueriable = sort.SortDirection switch
                    {
                        RemoteLinq.SortDirection.Ascending => orderedQueriable.ThenBy(exp),
                        RemoteLinq.SortDirection.Descending => orderedQueriable.ThenByDescending(exp),
                        _ => throw new InvalidOperationException($"Invalid {nameof(RemoteLinq.SortDirection)} '{sort.SortDirection}'"),
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
    }
}