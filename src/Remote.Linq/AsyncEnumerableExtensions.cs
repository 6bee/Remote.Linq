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
        public static Task<TSource> AggregateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Aggregate, source, func, cancellationToken);

        public static Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TAccumulate>(
                MethodInfos.Queryable.AggregateWithSeed,
                source,
                new object[] { seed, func },
                cancellationToken);

        public static Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TResult>(
                MethodInfos.Queryable.AggregateWithSeedAndSelector.MakeGenericMethod(typeof(TSource), typeof(TAccumulate), typeof(TResult)),
                source,
                new object[] { seed, func, selector },
                cancellationToken);

        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToList();
        }

        public static async Task<TSource[]> ToArrayAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToArray();
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector);
        }

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, comparer);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default)
        {
            var enumerator = await ExecuteAsync(source, cancellationToken).ConfigureAwait(false);
            return enumerator.ToDictionary(keySelector, elementSelector, comparer);
        }

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.First, source, cancellationToken);

        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstOrDefault, source, cancellationToken);

        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.FirstOrDefaultWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Single, source, cancellationToken);

        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefault, source, cancellationToken);

        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.SingleOrDefaultWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Last, source, cancellationToken);

        public static Task<TSource> LastAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefault, source, cancellationToken);

        public static Task<TSource> LastOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.LastOrDefaultWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> ElementAtAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAt, source, index, cancellationToken);

        public static Task<TSource> ElementAtOrDefaultAsync<TSource>(this IQueryable<TSource> source, int index, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.ElementAtOrDefault, source, index, cancellationToken);

        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Contains, source, item, cancellationToken);

        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.ContainsWithComparer,
                source,
                new object[] { item, comparer },
                cancellationToken);

        public static Task<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.SequenceEqualWithComparer, source1, source2, cancellationToken);

        public static Task<bool> SequenceEqualAsync<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(
                MethodInfos.Queryable.SequenceEqualWithComparer,
                source1,
                new object[] { source2, comparer },
                cancellationToken);

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.Any, source, cancellationToken);

        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.AnyWithPredicate, source, predicate, cancellationToken);

        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, bool>(MethodInfos.Queryable.All, source, predicate, cancellationToken);

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.Count, source, cancellationToken);

        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.CountWithPredicate, source, predicate, cancellationToken);

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCount, source, cancellationToken);

        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.LongCountWithPredicate, source, predicate, cancellationToken);

        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Min, source, cancellationToken);

        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TResult>(MethodInfos.Queryable.MinWithSelector, source, selector, cancellationToken);

        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TSource>(MethodInfos.Queryable.Max, source, cancellationToken);

        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, TResult>(MethodInfos.Queryable.MaxWithSelector, source, selector, cancellationToken);

        public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int, int>(MethodInfos.Queryable.SumInt32, source, cancellationToken);

        public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int?, int?>(MethodInfos.Queryable.SumNullableInt32, source, cancellationToken);

        public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long, long>(MethodInfos.Queryable.SumNullableInt64, source, cancellationToken);

        public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long?, long?>(MethodInfos.Queryable.SumNullableInt64, source, cancellationToken);

        public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.SumSingle, source, cancellationToken);

        public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.SumNullableSingle, source, cancellationToken);

        public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.SumDouble, source, cancellationToken);

        public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.SumNullableDouble, source, cancellationToken);

        public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.SumDecimal, source, cancellationToken);

        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.SumNullableDecimal, source, cancellationToken);

        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, int>(MethodInfos.Queryable.SumWithInt32Selector, source, selector, cancellationToken);

        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, int?>(MethodInfos.Queryable.SumWithNullableInt32Selector, source, selector, cancellationToken);

        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, long>(MethodInfos.Queryable.SumWithInt64Selector, source, selector, cancellationToken);

        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, long?>(MethodInfos.Queryable.SumWithNullableInt64Selector, source, selector, cancellationToken);

        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.SumWithSingleSelector, source, selector, cancellationToken);

        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.SumWithNullableSingleSelector, source, selector, cancellationToken);

        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.SumWithDoubleSelector, source, selector, cancellationToken);

        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.SumWithNullableDoubleSelector, source, selector, cancellationToken);

        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.SumWithDecimalSelector, source, selector, cancellationToken);

        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.SumWithNullableDecimalSelector, source, selector, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int, double>(MethodInfos.Queryable.AverageInt32, source, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<int?, double?>(MethodInfos.Queryable.AverageNullableInt32, source, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long, double>(MethodInfos.Queryable.AverageInt64, source, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<long?, double?>(MethodInfos.Queryable.AverageNullableInt64, source, cancellationToken);

        public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float, float>(MethodInfos.Queryable.AverageSingle, source, cancellationToken);

        public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<float?, float?>(MethodInfos.Queryable.AverageNullableSingle, source, cancellationToken);

        public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double, double>(MethodInfos.Queryable.AverageDouble, source, cancellationToken);

        public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<double?, double?>(MethodInfos.Queryable.AverageNullableDouble, source, cancellationToken);

        public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal, decimal>(MethodInfos.Queryable.AverageDecimal, source, cancellationToken);

        public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default)
            => ExecuteAsync<decimal?, decimal?>(MethodInfos.Queryable.AverageNullableDecimal, source, cancellationToken);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithInt32Selector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableInt32Selector, source, selector, cancellationToken);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithInt64Selector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableInt64Selector, source, selector, cancellationToken);

        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, float>(MethodInfos.Queryable.AverageWithSingleSelector, source, selector, cancellationToken);

        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, float?>(MethodInfos.Queryable.AverageWithNullableSingleSelector, source, selector, cancellationToken);

        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double>(MethodInfos.Queryable.AverageWithDoubleSelector, source, selector, cancellationToken);

        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, double?>(MethodInfos.Queryable.AverageWithNullableDoubleSelector, source, selector, cancellationToken);

        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, decimal>(MethodInfos.Queryable.AverageWithDecimalSelector, source, selector, cancellationToken);

        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default)
            => ExecuteAsync<TSource, decimal?>(MethodInfos.Queryable.AverageWithNullableDecimalSelector, source, selector, cancellationToken);

        private static async Task<IEnumerable<TSource>> ExecuteAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            if (source.Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                return await asyncQueryableProvider.ExecuteAsync<IEnumerable<TSource>>(source.Expression, cancellationToken).ConfigureAwait(false);
            }

            throw InvalidProviderException;
        }

        private static Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, CancellationToken cancellationToken)
            => ExecuteAsync<TSource, TResult>(method, source, Enumerable.Empty<object>(), cancellationToken);

        private static Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, object arg, CancellationToken cancellationToken)
            => ExecuteAsync<TSource, TResult>(method, source, new[] { arg }, cancellationToken);

        private static async Task<TResult> ExecuteAsync<TSource, TResult>(System.Reflection.MethodInfo method, IQueryable<TSource> source, IEnumerable<object> args, CancellationToken cancellationToken)
        {
            if (source.Provider is IAsyncRemoteQueryProvider asyncQueryableProvider)
            {
                if (method.IsGenericMethodDefinition)
                {
                    if (method.GetGenericArguments().Length > 2)
                    {
                        throw new Exception("Implementation error: expected closed generic method definition.");
                    }

                    method = method.GetGenericArguments().Length == 2
                        ? method.MakeGenericMethod(typeof(TSource), typeof(TResult))
                        : method.MakeGenericMethod(typeof(TSource));
                }

                var arguments = new[] { source.Expression }
                    .Concat(args.Select(x => x is Expression exp ? (Expression)Expression.Quote(exp) : Expression.Constant(x)))
                    .ToArray();
                var methodCallExpression = Expression.Call(null, method, arguments);

                return await asyncQueryableProvider.ExecuteAsync<TResult>(methodCallExpression, cancellationToken).ConfigureAwait(false);
            }

            throw InvalidProviderException;
        }

        private static InvalidOperationException InvalidProviderException
            => new InvalidOperationException($"The provider for the source IQueryable doesn't implement {nameof(IAsyncRemoteQueryProvider)}. Only providers implementing {typeof(IAsyncRemoteQueryProvider).FullName} can be used for Remote Linq asynchronous operations.");
    }
}
