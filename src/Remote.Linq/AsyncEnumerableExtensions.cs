// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToList();
        }

        public static async Task<T[]> ToArrayAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToArray();
        }

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToDictionary(keySelector);
        }

        public static async Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToDictionary(keySelector, comparer);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToDictionary(keySelector, elementSelector);
        }

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.ToDictionary(keySelector, elementSelector, comparer);
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.First();
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .FirstAsync();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Single();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .SingleAsync();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.SingleOrDefault();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .SingleOrDefaultAsync();
        }

        public static async Task<bool> ContainsAsync<T>(this IQueryable<T> source, T item)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Contains(item);
        }

        public static async Task<bool> AnyAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Any();
        }

        public static async Task<bool> AnyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .AnyAsync();
        }

        public static async Task<bool> AllAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            var unfilteredCount = source.CountAsync();
            var filteredCount = source.Where(predicate).CountAsync();
            return await unfilteredCount == await filteredCount;
        }

        public static async Task<int> CountAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Count();
        }

        public static async Task<int> CountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .CountAsync();
        }

        public static async Task<long> LongCountAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.LongCount();
        }

        public static async Task<long> LongCountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return await source
                .Where(predicate)
                .LongCountAsync();
        }

        public static async Task<T> MinAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Min();
        }

        public static async Task<TResult> MinAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector)
        {
            return await source
                .Select(selector)
                .MinAsync();
        }

        public static async Task<T> MaxAsync<T>(this IQueryable<T> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Max();
        }

        public static async Task<TResult> MaxAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector)
        {
            return await source
                .Select(selector)
                .MaxAsync();
        }

        public static async Task<int> SumAsync(this IQueryable<int> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<int?> SumAsync(this IQueryable<int?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<long> SumAsync(this IQueryable<long> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<long?> SumAsync(this IQueryable<long?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<float> SumAsync(this IQueryable<float> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<float?> SumAsync(this IQueryable<float?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<double> SumAsync(this IQueryable<double> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<double?> SumAsync(this IQueryable<double?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<decimal> SumAsync(this IQueryable<decimal> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<decimal?> SumAsync(this IQueryable<decimal?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Sum();
        }

        public static async Task<int> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<int?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<long> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<long?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<float> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<float?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<double> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<double?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<Decimal> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<Decimal?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal?>> selector)
        {
            return await source
                .Select(selector)
                .SumAsync();
        }

        public static async Task<double> AverageAsync(this IQueryable<int> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double?> AverageAsync(this IQueryable<int?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double> AverageAsync(this IQueryable<long> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double?> AverageAsync(this IQueryable<long?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<float> AverageAsync(this IQueryable<float> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<float?> AverageAsync(this IQueryable<float?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double> AverageAsync(this IQueryable<double> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double?> AverageAsync(this IQueryable<double?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<decimal> AverageAsync(this IQueryable<decimal> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<Decimal?> AverageAsync(this IQueryable<Decimal?> source)
        {
            var enumerator = await ExecuteAsync(source);
            return enumerator.Average();
        }

        public static async Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<float> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<float?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<Decimal> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        public static async Task<Decimal?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal?>> selector)
        {
            return await source
                .Select(selector)
                .AverageAsync();
        }

        private static async Task<IEnumerable<T>> ExecuteAsync<T>(IQueryable<T> source)
        {
            IEnumerable<T> enumerable;
            //if (source is IAsyncRemoteQueryable<T>)
            //{
            //    var asyncQueryable = (IAsyncRemoteQueryable<T>)source;
            //    enumerable = await asyncQueryable.ExecuteAsync();
            //}
            if (source.Provider is IAsyncRemoteQueryProvider)
            {
                var asyncQueryableProvider = (IAsyncRemoteQueryProvider)source.Provider;
                enumerable = await asyncQueryableProvider.ExecuteAsync<IEnumerable<T>>(source.Expression);
            }
            else
            {
                enumerable = await Task.Run(() => source.ToList()).ConfigureAwait(false);
            }

            return enumerable;
        }
    }
}
