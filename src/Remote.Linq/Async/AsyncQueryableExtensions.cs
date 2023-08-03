// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async
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
        public static ValueTask<TSource> AggregateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Aggregate, source, func, cancellation);

        public static ValueTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TAccumulate>(
                MethodInfos.Queryable.AggregateWithSeed,
                source,
                new object?[] { seed, func },
                cancellation);

        public static ValueTask<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult>(
                MethodInfos.Queryable.AggregateWithSeedAndSelector.MakeGenericMethod(typeof(TSource), typeof(TAccumulate), typeof(TResult)),
                source,
                new object?[] { seed, func, selector },
                cancellation);

        public static async ValueTask<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToList();
        }

        public static async ValueTask<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToArray();
        }

        public static async ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellation = default)
            where TKey : notnull
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector);
        }

        public static async ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellation = default)
            where TKey : notnull
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, comparer);
        }

        public static async ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellation = default)
            where TKey : notnull
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector);
        }

        public static async ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellation = default)
            where TKey : notnull
        {
            var enumerator = await source.ExecuteAsync(cancellation).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector, comparer);
        }

        public static ValueTask<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.First, source, cancellation);

        public static ValueTask<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstWithPredicate, source, predicate, cancellation);

        public static ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.FirstOrDefault, source, cancellation);

        public static ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.FirstOrDefaultWithPredicate, source, predicate, cancellation);

        // NET6.0 and later
        public static async ValueTask<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.FirstOrDefault, source, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        // NET6.0 and later
        public static async ValueTask<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.FirstOrDefaultWithPredicate, source, predicate, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        public static ValueTask<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Single, source, cancellation);

        public static ValueTask<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleWithPredicate, source, predicate, cancellation);

        public static ValueTask<TSource?> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.SingleOrDefault, source, cancellation);

        public static ValueTask<TSource?> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.SingleOrDefaultWithPredicate, source, predicate, cancellation);

        // NET6.0 and later
        public static async ValueTask<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefault, source, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        // NET6.0 and later
        public static async ValueTask<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefaultWithPredicate, source, predicate, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        public static ValueTask<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Last, source, cancellation);

        public static ValueTask<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastWithPredicate, source, predicate, cancellation);

        public static ValueTask<TSource?> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.LastOrDefault, source, cancellation);

        public static ValueTask<TSource?> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.LastOrDefaultWithPredicate, source, predicate, cancellation);

        // NET6.0 and later
        public static async ValueTask<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefault, source, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        // NET6.0 and later
        public static async ValueTask<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, TSource defaultValue, CancellationToken cancellation = default)
            => await ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefaultWithPredicate, source, predicate, cancellation).ConfigureAwait(false)
            ?? defaultValue;

        public static ValueTask<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAt, source, index, cancellation);

        public static ValueTask<TSource?> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.ElementAtOrDefault, source, index, cancellation);

#if NET6_0_OR_GREATER
        public static ValueTask<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, Index index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAt, source, index, cancellation);

        public static ValueTask<TSource?> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, Index index, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.ElementAtOrDefaultWithSystemIndex, source, index, cancellation);
#endif // NET6_0_OR_GREATER

        public static ValueTask<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Contains, source, item, cancellation);

        public static ValueTask<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.ContainsWithComparer,
                source,
                new object?[] { item, comparer },
                cancellation);

        public static ValueTask<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.SequenceEqual, source1, source2, cancellation);

        public static ValueTask<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.SequenceEqualWithComparer,
                source1,
                new object[] { source2, comparer },
                cancellation);

        public static ValueTask<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Any, source, cancellation);

        public static ValueTask<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.AnyWithPredicate, source, predicate, cancellation);

        public static ValueTask<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.All, source, predicate, cancellation);

        public static ValueTask<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.Count, source, cancellation);

        public static ValueTask<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.CountWithPredicate, source, predicate, cancellation);

        public static ValueTask<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCount, source, cancellation);

        public static ValueTask<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCountWithPredicate, source, predicate, cancellation);

        public static ValueTask<TSource?> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.Min, source, cancellation);

        public static ValueTask<TResult?> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult?>(MethodInfos.Queryable.MinWithSelector, source, selector, cancellation);

#if NET6_0_OR_GREATER
        public static ValueTask<TSource?> MinAsync<TSource>(this IQueryable<TSource> source, IComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MinWithComparer, source, comparer, cancellation);

        public static ValueTask<TSource?> MinByAsync<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MinBy, source, keySelector, cancellation);

        public static ValueTask<TSource?> MinByAsync<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TSource> comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MinByWithComparer, source, new object?[] { keySelector, comparer }, cancellation);
#endif // NET6_0_OR_GREATER

        public static ValueTask<TSource?> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.Max, source, cancellation);

        public static ValueTask<TResult?> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TResult?>(MethodInfos.Queryable.MaxWithSelector, source, selector, cancellation);

#if NET6_0_OR_GREATER
        public static ValueTask<TSource?> MaxAsync<TSource>(this IQueryable<TSource> source, IComparer<TSource>? comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MaxWithComparer, source, comparer, cancellation);

        public static ValueTask<TSource?> MaxByAsync<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MaxBy, source, keySelector, cancellation);

        public static ValueTask<TSource?> MaxByAsync<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TSource>? comparer, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, TSource?>(MethodInfos.Queryable.MaxByWithComparer, source, new object?[] { keySelector, comparer }, cancellation);
#endif // NET6_0_OR_GREATER

        public static ValueTask<int> SumAsync(this IQueryable<int> source, CancellationToken cancellation = default)
            => ExecuteAsync<int, int>(MethodInfos.Queryable.SumInt32, source, cancellation);

        public static ValueTask<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellation = default)
            => ExecuteAsync<int?, int?>(MethodInfos.Queryable.SumNullableInt32, source, cancellation);

        public static ValueTask<long> SumAsync(this IQueryable<long> source, CancellationToken cancellation = default)
            => ExecuteAsync<long, long>(MethodInfos.Queryable.SumInt64, source, cancellation);

        public static ValueTask<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellation = default)
            => ExecuteAsync<long?, long?>(MethodInfos.Queryable.SumNullableInt64, source, cancellation);

        public static ValueTask<float> SumAsync(this IQueryable<float> source, CancellationToken cancellation = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.SumSingle, source, cancellation);

        public static ValueTask<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellation = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.SumNullableSingle, source, cancellation);

        public static ValueTask<double> SumAsync(this IQueryable<double> source, CancellationToken cancellation = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.SumDouble, source, cancellation);

        public static ValueTask<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellation = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.SumNullableDouble, source, cancellation);

        public static ValueTask<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.SumDecimal, source, cancellation);

        public static ValueTask<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.SumNullableDecimal, source, cancellation);

        public static ValueTask<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.SumWithInt32Selector, source, selector, cancellation);

        public static ValueTask<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, int?>(MethodInfos.Queryable.SumWithNullableInt32Selector, source, selector, cancellation);

        public static ValueTask<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.SumWithInt64Selector, source, selector, cancellation);

        public static ValueTask<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, long?>(MethodInfos.Queryable.SumWithNullableInt64Selector, source, selector, cancellation);

        public static ValueTask<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.SumWithSingleSelector, source, selector, cancellation);

        public static ValueTask<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.SumWithNullableSingleSelector, source, selector, cancellation);

        public static ValueTask<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.SumWithDoubleSelector, source, selector, cancellation);

        public static ValueTask<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.SumWithNullableDoubleSelector, source, selector, cancellation);

        public static ValueTask<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.SumWithDecimalSelector, source, selector, cancellation);

        public static ValueTask<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.SumWithNullableDecimalSelector, source, selector, cancellation);

        public static ValueTask<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellation = default)
            => ExecuteAsync<int, double>(MethodInfos.Queryable.AverageInt32, source, cancellation);

        public static ValueTask<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellation = default)
            => ExecuteAsync<int?, double?>(MethodInfos.Queryable.AverageNullableInt32, source, cancellation);

        public static ValueTask<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellation = default)
            => ExecuteAsync<long, double>(MethodInfos.Queryable.AverageInt64, source, cancellation);

        public static ValueTask<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellation = default)
            => ExecuteAsync<long?, double?>(MethodInfos.Queryable.AverageNullableInt64, source, cancellation);

        public static ValueTask<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellation = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.AverageSingle, source, cancellation);

        public static ValueTask<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellation = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.AverageNullableSingle, source, cancellation);

        public static ValueTask<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellation = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.AverageDouble, source, cancellation);

        public static ValueTask<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellation = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.AverageNullableDouble, source, cancellation);

        public static ValueTask<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.AverageDecimal, source, cancellation);

        public static ValueTask<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellation = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.AverageNullableDecimal, source, cancellation);

        public static ValueTask<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageInt32WithSelector, source, selector, cancellation);

        public static ValueTask<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageNullableInt32WithSelector, source, selector, cancellation);

        public static ValueTask<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageInt64WithSelector, source, selector, cancellation);

        public static ValueTask<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageNullableInt64WithSelector, source, selector, cancellation);

        public static ValueTask<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.AverageSingleWithSelector, source, selector, cancellation);

        public static ValueTask<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.AverageNullableSingleWithSelector, source, selector, cancellation);

        public static ValueTask<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageDoubleWithSelector, source, selector, cancellation);

        public static ValueTask<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageNullableDoubleWithSelector, source, selector, cancellation);

        public static ValueTask<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.AverageDecimalWithSelector, source, selector, cancellation);

        public static ValueTask<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellation = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.AverageNullableDecimalWithSelector, source, selector, cancellation);

        /// <summary>Returns an <see cref="IAsyncEnumerable{TSource}" /> which can be enumerated asynchronously.</summary>
        /// <param name="source">An <see cref="IQueryable{TSource}" /> to enumerate.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> allowing cancellation of the async stream execution.</param>
        /// <returns>The query results.</returns>
        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
        {
            source.AssertNotNull();

            if (source is IAsyncEnumerable<TSource> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            if (source.Provider is IAsyncRemoteStreamProvider asyncRemoteStreamProvider)
            {
                return asyncRemoteStreamProvider.ExecuteAsyncRemoteStream<TSource>(source.Expression, cancellation);
            }

            if (source.Provider is IAsyncRemoteQueryProvider asyncRemoteQueryProvider)
            {
                var asyncResult = asyncRemoteQueryProvider.ExecuteAsync<IEnumerable<TSource>>(source.Expression, cancellation);
                return asyncResult.ToAsyncEnumerable();
            }

            if (source.Provider is IRemoteQueryProvider remoteQueryProvider)
            {
                var result = remoteQueryProvider.Execute<IEnumerable<TSource>>(source.Expression);
                return new ValueTask<IEnumerable<TSource>>(result).ToAsyncEnumerable();
            }

            return new ValueTask<IEnumerable<TSource>>(source).ToAsyncEnumerable();
        }

        private static async IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(this ValueTask<IEnumerable<TSource>> source)
        {
            foreach (var item in await source.ConfigureAwait(false))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Executes the async remote queryable.
        /// </summary>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous result of the remote query.</returns>
        public static async ValueTask<TResult> ExecuteAsync<TResult>(this IQueryable source, CancellationToken cancellation = default)
        {
            if (source.CheckNotNull().Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                return await asyncQueryableProvider.ExecuteAsync<TResult>(source.Expression, cancellation).ConfigureAwait(false);
            }

            return source.Provider.Execute<TResult>(source.Expression);
        }

        internal static ValueTask<IEnumerable<TSource>> ExecuteAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation = default)
            => source.ExecuteAsync<IEnumerable<TSource>>(cancellation);

        private static ValueTask<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, CancellationToken cancellation)
            => ExecuteAsync<TSource, TResult>(method, source, Enumerable.Empty<object>(), cancellation);

        private static ValueTask<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, object? arg, CancellationToken cancellation)
            => ExecuteAsync<TSource, TResult>(method, source, new[] { arg }, cancellation);

        private static async ValueTask<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, IEnumerable<object?> args, CancellationToken cancellation)
        {
            source.AssertNotNull();

            if (method.IsGenericMethodDefinition)
            {
                var genericArgumentCount = method.GetGenericArguments().Length;
                if (genericArgumentCount > 2)
                {
                    throw new RemoteLinqException("Implementation error: expected closed generic method definition.");
                }

                method = genericArgumentCount == 2
                    ? method.MakeGenericMethod(typeof(TSource), typeof(TResult))
                    : method.MakeGenericMethod(typeof(TSource));
            }

            var arguments = new[] { source.Expression }
                .Concat(args.Select(x => x is Expression exp ? (Expression)Expression.Quote(exp) : Expression.Constant(x)))
                .ToArray();
            var methodCallExpression = Expression.Call(method, arguments);

            if (source.Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                return await asyncQueryableProvider.ExecuteAsync<TResult>(methodCallExpression, cancellation).ConfigureAwait(false);
            }

            return source.Provider.Execute<TResult>(methodCallExpression);
        }
    }
}