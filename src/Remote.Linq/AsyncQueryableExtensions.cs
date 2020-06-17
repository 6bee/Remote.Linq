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
    public static class AsyncQueryableExtensions
    {
        private static InvalidOperationException NotAsyncRemoteQueryProviderException
            => new InvalidOperationException($"The provider for the source IQueryable doesn't implement {nameof(IAsyncRemoteQueryProvider)}. Only providers implementing {typeof(IAsyncRemoteQueryProvider).FullName} can be used for Remote Linq asynchronous operations.");

        private static InvalidOperationException NotAsyncRemoteStreamProviderException
            => new InvalidOperationException($"The provider for the source IQueryable doesn't implement {nameof(IAsyncRemoteStreamProvider)}. Only providers implementing {typeof(IAsyncRemoteStreamProvider).FullName} can be used for Remote Linq's {nameof(AsAsyncEnumerable)} operation.");

        public static Task<TSource> AggregateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Aggregate, source, func, cancellation);

        public static Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TAccumulate>(
                MethodInfos.Queryable.AggregateWithSeed,
                source,
                new object?[] { seed, func },
                cancellation);

        public static Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult>(
                MethodInfos.Queryable.AggregateWithSeedAndSelector.MakeGenericMethod(typeof(TSource), typeof(TAccumulate), typeof(TResult)),
                source,
                new object?[] { seed, func, selector },
                cancellation);

        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToList();
        }

        public static async Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToArray();
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector);
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, comparer);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellation = default)
        {
            var enumerator = await ExecuteAsync(source, cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector, comparer);
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.First, source, cancellation);

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstWithPredicate, source, predicate, cancellation);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstOrDefault, source, cancellation);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstOrDefaultWithPredicate, source, predicate, cancellation);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Single, source, cancellation);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleWithPredicate, source, predicate, cancellation);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefault, source, cancellation);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefaultWithPredicate, source, predicate, cancellation);

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Last, source, cancellation);

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastWithPredicate, source, predicate, cancellation);

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefault, source, cancellation);

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefaultWithPredicate, source, predicate, cancellation);

        public static Task<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAt, source, index, cancellation);

        public static Task<TSource> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAtOrDefault, source, index, cancellation);

        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Contains, source, item, cancellation);

        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.ContainsWithComparer,
                source,
                new object?[] { item, comparer },
                cancellation);

        public static Task<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.SequenceEqualWithComparer, source1, source2, cancellation);

        public static Task<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.SequenceEqualWithComparer,
                source1,
                new object[] { source2, comparer },
                cancellation);

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Any, source, cancellation);

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.AnyWithPredicate, source, predicate, cancellation);

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.All, source, predicate, cancellation);

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.Count, source, cancellation);

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.CountWithPredicate, source, predicate, cancellation);

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCount, source, cancellation);

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCountWithPredicate, source, predicate, cancellation);

        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Min, source, cancellation);

        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult>(MethodInfos.Queryable.MinWithSelector, source, selector, cancellation);

        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Max, source, cancellation);

        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult>(MethodInfos.Queryable.MaxWithSelector, source, selector, cancellation);

        public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellation = default)
            => ExecuteAsync<int, int>(MethodInfos.Queryable.SumInt32, source, cancellation);

        public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellation = default)
            => ExecuteAsync<int?, int?>(MethodInfos.Queryable.SumNullableInt32, source, cancellation);

        public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellation = default)
            => ExecuteAsync<long, long>(MethodInfos.Queryable.SumNullableInt64, source, cancellation);

        public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellation = default)
            => ExecuteAsync<long?, long?>(MethodInfos.Queryable.SumNullableInt64, source, cancellation);

        public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellation = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.SumSingle, source, cancellation);

        public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellation = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.SumNullableSingle, source, cancellation);

        public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellation = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.SumDouble, source, cancellation);

        public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellation = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.SumNullableDouble, source, cancellation);

        public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.SumDecimal, source, cancellation);

        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.SumNullableDecimal, source, cancellation);

        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.SumWithInt32Selector, source, selector, cancellation);

        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int?>(MethodInfos.Queryable.SumWithNullableInt32Selector, source, selector, cancellation);

        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.SumWithInt64Selector, source, selector, cancellation);

        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long?>(MethodInfos.Queryable.SumWithNullableInt64Selector, source, selector, cancellation);

        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.SumWithSingleSelector, source, selector, cancellation);

        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.SumWithNullableSingleSelector, source, selector, cancellation);

        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.SumWithDoubleSelector, source, selector, cancellation);

        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.SumWithNullableDoubleSelector, source, selector, cancellation);

        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.SumWithDecimalSelector, source, selector, cancellation);

        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.SumWithNullableDecimalSelector, source, selector, cancellation);

        public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellation = default)
            => ExecuteAsync<int, double>(MethodInfos.Queryable.AverageInt32, source, cancellation);

        public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellation = default)
            => ExecuteAsync<int?, double?>(MethodInfos.Queryable.AverageNullableInt32, source, cancellation);

        public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellation = default)
            => ExecuteAsync<long, double>(MethodInfos.Queryable.AverageInt64, source, cancellation);

        public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellation = default)
            => ExecuteAsync<long?, double?>(MethodInfos.Queryable.AverageNullableInt64, source, cancellation);

        public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellation = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.AverageSingle, source, cancellation);

        public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellation = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.AverageNullableSingle, source, cancellation);

        public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellation = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.AverageDouble, source, cancellation);

        public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellation = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.AverageNullableDouble, source, cancellation);

        public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.AverageDecimal, source, cancellation);

        public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.AverageNullableDecimal, source, cancellation);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithInt32Selector, source, selector, cancellation);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableInt32Selector, source, selector, cancellation);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithInt64Selector, source, selector, cancellation);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableInt64Selector, source, selector, cancellation);

        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.AverageWithSingleSelector, source, selector, cancellation);

        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.AverageWithNullableSingleSelector, source, selector, cancellation);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithDoubleSelector, source, selector, cancellation);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableDoubleSelector, source, selector, cancellation);

        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.AverageWithDecimalSelector, source, selector, cancellation);

        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.AverageWithNullableDecimalSelector, source, selector, cancellation);

        /// <summary>Returns an <see cref="IAsyncEnumerable{TSource}" /> which can be enumerated asynchronously.</summary>
        /// <param name="source">An <see cref="IQueryable{TSource}" /> to enumerate.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> allowing cancellation of the async stream execution.</param>
        /// <returns>The query results.</returns>
        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            if (source.CheckNotNull(nameof(source)).Provider is IAsyncRemoteStreamProvider asyncRemoteStreamProvider)
            {
                return asyncRemoteStreamProvider.ExecuteAsyncRemoteStream<TSource>(source.Expression, cancellation);
            }

            throw NotAsyncRemoteStreamProviderException;
        }

        public static async Task<TResult> ExecuteAsync<TResult>(this IQueryable source, CancellationToken cancellation = default)
        {
            if (source.CheckNotNull(nameof(source)).Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                return await asyncQueryableProvider.ExecuteAsync<TResult>(source.Expression, cancellation).ConfigureAwait(false);
            }

            throw NotAsyncRemoteQueryProviderException;
        }

        internal static Task<IEnumerable<TSource>> ExecuteAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<IEnumerable<TSource>>(source, cancellation);

        private static Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, CancellationToken cancellation)
            => ExecuteAsync<TSource, TResult>(method, source, Enumerable.Empty<object>(), cancellation);

        private static Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, object? arg, CancellationToken cancellation)
            => ExecuteAsync<TSource, TResult>(method, source, new[] { arg }, cancellation);

        private static async Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, IEnumerable<object?> args, CancellationToken cancellation)
        {
            if (source.CheckNotNull(nameof(source)).Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                if (method.IsGenericMethodDefinition)
                {
                    if (method.GetGenericArguments().Length > 2)
                    {
                        throw new RemoteLinqException("Implementation error: expected closed generic method definition.");
                    }

                    method = method.GetGenericArguments().Length == 2
                        ? method.MakeGenericMethod(typeof(TSource), typeof(TResult))
                        : method.MakeGenericMethod(typeof(TSource));
                }

                var arguments = new[] { source.Expression }
                    .Concat(args.Select(x => x is Expression exp ? (Expression)Expression.Quote(exp) : Expression.Constant(x)))
                    .ToArray();
                var methodCallExpression = Expression.Call(null, method, arguments);

                return await asyncQueryableProvider.ExecuteAsync<TResult>(methodCallExpression, cancellation).ConfigureAwait(false);
            }

            throw NotAsyncRemoteQueryProviderException;
        }
    }
}
