// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AsyncEnumerableExtensions
    {
        // TODO: write test to ensure every queryable operation has corresponding async implementation
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToList();
        }

        public static async Task<T[]> ToArrayAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToArray();
        }

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToDictionary(keySelector);
        }

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToDictionary(keySelector, comparer);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToDictionary(keySelector, elementSelector);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken);
            return enumerator.ToDictionary(keySelector, elementSelector, comparer);
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.First, source, null, cancellationToken);

        public static Task<T> FirstAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.FirstWithPredicate, source, predicate, cancellationToken);

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.FirstOrDefault, source, null, cancellationToken);

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.FirstOrDefaultWithPredicate, source, predicate, cancellationToken);

        public static Task<T> SingleAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.Single, source, null, cancellationToken);

        public static Task<T> SingleAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.SingleWithPredicate, source, predicate, cancellationToken);

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.SingleOrDefault, source, null, cancellationToken);

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.SingleOrDefaultWithPredicate, source, predicate, cancellationToken);

        public static Task<bool> ContainsAsync<T>(this IQueryable<T> source, T item, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, bool>(MethodInfos.Queryable.Contains, source, Expression.Constant(item), cancellationToken);

        public static Task<bool> AnyAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, bool>(MethodInfos.Queryable.Any, source, null, cancellationToken);

        public static Task<bool> AnyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, bool>(MethodInfos.Queryable.AnyWithPredicate, source, predicate, cancellationToken);

        public static Task<bool> AllAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, bool>(MethodInfos.Queryable.All, source, predicate, cancellationToken);

        public static Task<int> CountAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, int>(MethodInfos.Queryable.Count, source, null, cancellationToken);

        public static Task<int> CountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, int>(MethodInfos.Queryable.CountWithPredicate, source, predicate, cancellationToken);

        public static Task<long> LongCountAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, long>(MethodInfos.Queryable.LongCount, source, null, cancellationToken);

        public static Task<long> LongCountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, long>(MethodInfos.Queryable.LongCountWithPredicate, source, predicate, cancellationToken);

        public static Task<T> MinAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.Min, source, null, cancellationToken);

        public static Task<TResult> MinAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, TResult>(MethodInfos.Queryable.MinWithSelector, source, selector, cancellationToken);

        public static Task<T> MaxAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, T>(MethodInfos.Queryable.Max, source, null, cancellationToken);

        public static Task<TResult> MaxAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, TResult>(MethodInfos.Queryable.MaxWithSelector, source, selector, cancellationToken);

        public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int, int>(MethodInfos.Queryable.SumInt32, source, null, cancellationToken);

        public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int?, int?>(MethodInfos.Queryable.SumNullableInt32, source, null, cancellationToken);

        public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long, long>(MethodInfos.Queryable.SumNullableInt64, source, null, cancellationToken);

        public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long?, long?>(MethodInfos.Queryable.SumNullableInt64, source, null, cancellationToken);

        public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.SumSingle, source, null, cancellationToken);

        public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.SumNullableSingle, source, null, cancellationToken);

        public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.SumDouble, source, null, cancellationToken);

        public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.SumNullableDouble, source, null, cancellationToken);

        public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.SumDecimal, source, null, cancellationToken);

        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.SumNullableDecimal, source, null, cancellationToken);

        public static Task<int> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, int>(MethodInfos.Queryable.SumWithInt32Selector, source, selector, cancellationToken);

        public static Task<int?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, int?>(MethodInfos.Queryable.SumWithNullableInt32Selector, source, selector, cancellationToken);

        public static Task<long> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, long>(MethodInfos.Queryable.SumWithInt64Selector, source, selector, cancellationToken);

        public static Task<long?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, long?>(MethodInfos.Queryable.SumWithNullableInt64Selector, source, selector, cancellationToken);

        public static Task<float> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, float>(MethodInfos.Queryable.SumWithSingleSelector, source, selector, cancellationToken);

        public static Task<float?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, float?>(MethodInfos.Queryable.SumWithNullableSingleSelector, source, selector, cancellationToken);

        public static Task<double> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double>(MethodInfos.Queryable.SumWithDoubleSelector, source, selector, cancellationToken);

        public static Task<double?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double?>(MethodInfos.Queryable.SumWithNullableDoubleSelector, source, selector, cancellationToken);

        public static Task<decimal> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, decimal>(MethodInfos.Queryable.SumWithDecimalSelector, source, selector, cancellationToken);

        public static Task<decimal?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, decimal?>(MethodInfos.Queryable.SumWithNullableDecimalSelector, source, selector, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int, double>(MethodInfos.Queryable.AverageInt32, source, null, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int?, double?>(MethodInfos.Queryable.AverageNullableInt32, source, null, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long, double>(MethodInfos.Queryable.AverageInt64, source, null, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long?, double?>(MethodInfos.Queryable.AverageNullableInt64, source, null, cancellationToken);

        public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.AverageSingle, source, null, cancellationToken);

        public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.AverageNullableSingle, source, null, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.AverageDouble, source, null, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.AverageNullableDouble, source, null, cancellationToken);

        public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.AverageDecimal, source, null, cancellationToken);

        public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.AverageNullableDecimal, source, null, cancellationToken);

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double>(MethodInfos.Queryable.AverageWithInt32Selector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double?>(MethodInfos.Queryable.AverageWithNullableInt32Selector, source, selector, cancellationToken);

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double>(MethodInfos.Queryable.AverageWithInt64Selector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double?>(MethodInfos.Queryable.AverageWithNullableInt64Selector, source, selector, cancellationToken);

        public static Task<float> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, float>(MethodInfos.Queryable.AverageWithSingleSelector, source, selector, cancellationToken);

        public static Task<float?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, float?>(MethodInfos.Queryable.AverageWithNullableSingleSelector, source, selector, cancellationToken);

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double>(MethodInfos.Queryable.AverageWithDoubleSelector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, double?>(MethodInfos.Queryable.AverageWithNullableDoubleSelector, source, selector, cancellationToken);

        public static Task<decimal> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, decimal>(MethodInfos.Queryable.AverageWithDecimalSelector, source, selector, cancellationToken);

        public static Task<decimal?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<T, decimal?>(MethodInfos.Queryable.AverageWithNullableDecimalSelector, source, selector, cancellationToken);

        private static async Task<IEnumerable<T>> ExecuteAsync<T>(IQueryable<T> source, CancellationToken cancellationToken)
        {
            if (source.Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                return await asyncQueryableProvider.ExecuteAsync<IEnumerable<T>>(source.Expression, cancellationToken);
            }

            throw InvalidProviderException;
        }

        private static async Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, Expression arg, CancellationToken cancellationToken)
        {
            if (source.Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                if (method.IsGenericMethod)
                {
                    method = method.GetGenericArguments().Length == 2
                        ? method.MakeGenericMethod(typeof(TSource), typeof(TResult))
                        : method.MakeGenericMethod(typeof(TSource));
                }

                Expression[] arguments = arg is null
                    ? new[] { source.Expression }
                    : new[] { source.Expression, arg };

                var methodCallExpression = Expression.Call(null, method, arguments);

                return await asyncQueryableProvider.ExecuteAsync<TResult>(methodCallExpression, cancellationToken);
            }

            throw InvalidProviderException;
        }

        private static InvalidOperationException InvalidProviderException
            => new InvalidOperationException($"The provider for the source IQueryable doesn't implement {nameof(IAsyncRemoteQueryProvider)}. Only providers that implement {nameof(IAsyncRemoteQueryProvider)} can be used for Remote Linq asynchronous operations.");
    }
}
