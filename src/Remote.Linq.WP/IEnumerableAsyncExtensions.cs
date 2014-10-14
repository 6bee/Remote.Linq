// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Remote.Linq
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IEnumerableAsyncExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.ToList());
        }
        public static Task<T[]> ToArrayAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.ToArray());
        }

        public static Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector)
        {
            return ExecuteAsync(source, x => x.ToDictionary(keySelector));
        }

        public static Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(this IQueryable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return ExecuteAsync(source, x => x.ToDictionary(keySelector, comparer));
        }

        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        {
            return ExecuteAsync(source, x => x.ToDictionary(keySelector, elementSelector));
        }

        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(this IQueryable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return ExecuteAsync(source, x => x.ToDictionary(keySelector, elementSelector, comparer));
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.First());
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .FirstAsync();
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.FirstOrDefault());
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.Single());
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .SingleAsync();
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.SingleOrDefault());
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .SingleOrDefaultAsync();
        }

        public static Task<bool> ContainsAsync<T>(this IQueryable<T> source, T item)
        {
            return ExecuteAsync(source, x => x.Contains(item));
        }

        public static Task<bool> AnyAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.Any());
        }

        public static Task<bool> AnyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .AnyAsync();
        }

        public static Task<bool> AllAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            var unfilteredCount = source.CountAsync();
            var filteredCount = source.Where(predicate).CountAsync();

            return Task.Factory.StartNew(() => unfilteredCount.Result == filteredCount.Result);
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.Count());
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .CountAsync();
        }

        public static Task<long> LongCountAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.LongCount());
        }

        public static Task<long> LongCountAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source
                .Where(predicate)
                .LongCountAsync();
        }

        public static Task<T> MinAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.Min());
        }

        public static Task<TResult> MinAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector)
        {
            return source
                .Select(selector)
                .MinAsync();
        }

        public static Task<T> MaxAsync<T>(this IQueryable<T> source)
        {
            return ExecuteAsync(source, x => x.Max());
        }

        public static Task<TResult> MaxAsync<T, TResult>(this IQueryable<T> source, Expression<Func<T, TResult>> selector)
        {
            return source
                .Select(selector)
                .MaxAsync();
        }

        public static Task<int> SumAsync(this IQueryable<int> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<int?> SumAsync(this IQueryable<int?> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<long> SumAsync(this IQueryable<long> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<long?> SumAsync(this IQueryable<long?> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<float> SumAsync(this IQueryable<float> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<float?> SumAsync(this IQueryable<float?> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<double> SumAsync(this IQueryable<double> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<double?> SumAsync(this IQueryable<double?> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<decimal> SumAsync(this IQueryable<decimal> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<decimal?> SumAsync(this IQueryable<decimal?> source)
        {
            return ExecuteAsync(source, x => x.Sum());
        }

        public static Task<int> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<int?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<long> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<long?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<float> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<float?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<double> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<double?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<Decimal> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<Decimal?> SumAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal?>> selector)
        {
            return source
                .Select(selector)
                .SumAsync();
        }

        public static Task<double> AverageAsync(this IQueryable<int> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double?> AverageAsync(this IQueryable<int?> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double> AverageAsync(this IQueryable<long> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double?> AverageAsync(this IQueryable<long?> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<float> AverageAsync(this IQueryable<float> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<float?> AverageAsync(this IQueryable<float?> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double> AverageAsync(this IQueryable<double> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double?> AverageAsync(this IQueryable<double?> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<decimal> AverageAsync(this IQueryable<decimal> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<Decimal?> AverageAsync(this IQueryable<Decimal?> source)
        {
            return ExecuteAsync(source, x => x.Average());
        }

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, int?>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, long?>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<float> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<float?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, float?>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<double> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<double?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, double?>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<Decimal> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        public static Task<Decimal?> AverageAsync<T>(this IQueryable<T> source, Expression<Func<T, Decimal?>> selector)
        {
            return source
                .Select(selector)
                .AverageAsync();
        }

        private static Task<TResult> ExecuteAsync<T, TResult>(IQueryable<T> source, Func<IEnumerable<T>,TResult> projection)
        {
            Task<IEnumerable<T>> task;
            if (source is IAsyncQueryable<T>)
            {
                var asyncQueryable = (IAsyncQueryable<T>)source;
                task = asyncQueryable.ExecuteAsync();
            }
            else
            {
                task = Task.Factory.StartNew(() => source.ToList().AsEnumerable());
            }

            var result = task.ContinueWith(t => projection(t.Result));
            return result;
        }
    }
}
