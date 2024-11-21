// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

partial class MethodInfos
{
    internal static class Queryable
    {
        internal static readonly MethodInfo AsQueryable = GetQueryableMethod(
            nameof(System.Linq.Queryable.AsQueryable),
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo OrderBy = GetQueryableMethod(
            nameof(System.Linq.Queryable.OrderBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo OrderByDescending = GetQueryableMethod(
            nameof(System.Linq.Queryable.OrderByDescending),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo ThenBy = GetQueryableMethod(
            nameof(System.Linq.Queryable.ThenBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IOrderedQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo ThenByDescending = GetQueryableMethod(
            nameof(System.Linq.Queryable.ThenByDescending),
            [typeof(TSource), typeof(TKey)],
            typeof(IOrderedQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo Aggregate = GetQueryableMethod(
            nameof(System.Linq.Queryable.Aggregate),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TSource, TSource>>));

        internal static readonly MethodInfo AggregateWithSeed = GetQueryableMethod(
            nameof(System.Linq.Queryable.Aggregate),
            [typeof(TSource), typeof(TAccumulate)],
            typeof(IQueryable<TSource>),
            typeof(TAccumulate),
            typeof(Expression<Func<TAccumulate, TSource, TAccumulate>>));

        internal static readonly MethodInfo AggregateWithSeedAndSelector = GetQueryableMethod(
            nameof(System.Linq.Queryable.Aggregate),
            [typeof(TSource), typeof(TAccumulate), typeof(TResult)],
            typeof(IQueryable<TSource>),
            typeof(TAccumulate),
            typeof(Expression<Func<TAccumulate, TSource, TAccumulate>>),
            typeof(Expression<Func<TAccumulate, TResult>>));

        internal static readonly MethodInfo All = GetQueryableMethod(
            nameof(System.Linq.Queryable.All),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo Any = GetQueryableMethod(
            nameof(System.Linq.Queryable.Any),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo AnyWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.Any),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo AverageInt32 = GetAverage<int>();
        internal static readonly MethodInfo AverageNullableInt32 = GetAverage<int?>();
        internal static readonly MethodInfo AverageInt64 = GetAverage<long>();
        internal static readonly MethodInfo AverageNullableInt64 = GetAverage<long?>();
        internal static readonly MethodInfo AverageSingle = GetAverage<float>();
        internal static readonly MethodInfo AverageNullableSingle = GetAverage<float?>();
        internal static readonly MethodInfo AverageDouble = GetAverage<double>();
        internal static readonly MethodInfo AverageNullableDouble = GetAverage<double?>();
        internal static readonly MethodInfo AverageDecimal = GetAverage<decimal>();
        internal static readonly MethodInfo AverageNullableDecimal = GetAverage<decimal?>();

        internal static readonly MethodInfo AverageInt32WithSelector = GetAverageWithSelector<int>();
        internal static readonly MethodInfo AverageNullableInt32WithSelector = GetAverageWithSelector<int?>();
        internal static readonly MethodInfo AverageSingleWithSelector = GetAverageWithSelector<float>();
        internal static readonly MethodInfo AverageNullableSingleWithSelector = GetAverageWithSelector<float?>();
        internal static readonly MethodInfo AverageInt64WithSelector = GetAverageWithSelector<long>();
        internal static readonly MethodInfo AverageNullableInt64WithSelector = GetAverageWithSelector<long?>();
        internal static readonly MethodInfo AverageDoubleWithSelector = GetAverageWithSelector<double>();
        internal static readonly MethodInfo AverageNullableDoubleWithSelector = GetAverageWithSelector<double?>();
        internal static readonly MethodInfo AverageDecimalWithSelector = GetAverageWithSelector<decimal>();
        internal static readonly MethodInfo AverageNullableDecimalWithSelector = GetAverageWithSelector<decimal?>();

        internal static readonly MethodInfo ContainsWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.Contains),
            typeof(IQueryable<TSource>),
            typeof(TSource),
            typeof(IEqualityComparer<TSource>));

        internal static readonly MethodInfo Contains = GetQueryableMethod(
            nameof(System.Linq.Queryable.Contains),
            typeof(IQueryable<TSource>),
            typeof(TSource));

        internal static readonly MethodInfo CountWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.Count),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo Count = GetQueryableMethod(
            nameof(System.Linq.Queryable.Count),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo ElementAt = GetQueryableMethod(
            nameof(System.Linq.Queryable.ElementAt),
            typeof(IQueryable<TSource>),
            typeof(int));

        internal static readonly MethodInfo ElementAtOrDefault = GetQueryableMethod(
            nameof(System.Linq.Queryable.ElementAtOrDefault),
            typeof(IQueryable<TSource>),
            typeof(int));

#if NET8_0_OR_GREATER
        internal static readonly MethodInfo ElementAtWithSystemIndex = GetQueryableMethod(
            nameof(System.Linq.Queryable.ElementAt),
            typeof(IQueryable<TSource>),
            typeof(Index));

        internal static readonly MethodInfo ElementAtOrDefaultWithSystemIndex = GetQueryableMethod(
            nameof(System.Linq.Queryable.ElementAtOrDefault),
            typeof(IQueryable<TSource>),
            typeof(Index));
#endif // NET8_0_OR_GREATER

        internal static readonly MethodInfo First = GetQueryableMethod(
            nameof(System.Linq.Queryable.First),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo FirstWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.First),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo FirstOrDefaultWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.FirstOrDefault),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo FirstOrDefault = GetQueryableMethod(
            nameof(System.Linq.Queryable.FirstOrDefault),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo Last = GetQueryableMethod(
            nameof(System.Linq.Queryable.Last),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo LastWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.Last),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo LastOrDefault = GetQueryableMethod(
            nameof(System.Linq.Queryable.LastOrDefault),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo LastOrDefaultWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.LastOrDefault),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo LongCountWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.LongCount),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo LongCount = GetQueryableMethod(
            nameof(System.Linq.Queryable.LongCount),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo Max = GetQueryableMethod(
            nameof(System.Linq.Queryable.Max),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo MaxWithSelector = GetQueryableMethod(
            nameof(System.Linq.Queryable.Max),
            [typeof(TSource), typeof(TResult)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TResult>>));

#if NET8_0_OR_GREATER
        internal static readonly MethodInfo MaxWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.Max),
            [typeof(TSource)],
            typeof(IQueryable<TSource>),
            typeof(IComparer<TSource>));

        internal static readonly MethodInfo MaxBy = GetQueryableMethod(
            nameof(System.Linq.Queryable.MaxBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo MaxByWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.MaxBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>),
            typeof(IComparer<TSource>));
#endif // NET8_0_OR_GREATER

        internal static readonly MethodInfo Min = GetQueryableMethod(
            nameof(System.Linq.Queryable.Min),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo MinWithSelector = GetQueryableMethod(
            nameof(System.Linq.Queryable.Min),
            [typeof(TSource), typeof(TResult)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TResult>>));

#if NET8_0_OR_GREATER
        internal static readonly MethodInfo MinWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.Min),
            [typeof(TSource)],
            typeof(IQueryable<TSource>),
            typeof(IComparer<TSource>));

        internal static readonly MethodInfo MinBy = GetQueryableMethod(
            nameof(System.Linq.Queryable.MinBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>));

        internal static readonly MethodInfo MinByWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.MinBy),
            [typeof(TSource), typeof(TKey)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TKey>>),
            typeof(IComparer<TSource>));
#endif // NET8_0_OR_GREATER

        internal static readonly MethodInfo SequenceEqualWithComparer = GetQueryableMethod(
            nameof(System.Linq.Queryable.SequenceEqual),
            typeof(IQueryable<TSource>),
            typeof(IEnumerable<TSource>),
            typeof(IEqualityComparer<TSource>));

        internal static readonly MethodInfo SequenceEqual = GetQueryableMethod(
            nameof(System.Linq.Queryable.SequenceEqual),
            typeof(IQueryable<TSource>),
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo SingleWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.Single),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo Single = GetQueryableMethod(
            nameof(System.Linq.Queryable.Single),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo SingleOrDefault = GetQueryableMethod(
            nameof(System.Linq.Queryable.SingleOrDefault),
            typeof(IQueryable<TSource>));

        internal static readonly MethodInfo SingleOrDefaultWithPredicate = GetQueryableMethod(
            nameof(System.Linq.Queryable.SingleOrDefault),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, bool>>));

        internal static readonly MethodInfo SumWithInt32Selector = GetSumWithSelector<int>();
        internal static readonly MethodInfo SumWithNullableInt32Selector = GetSumWithSelector<int?>();
        internal static readonly MethodInfo SumWithInt64Selector = GetSumWithSelector<long>();
        internal static readonly MethodInfo SumWithNullableInt64Selector = GetSumWithSelector<long?>();
        internal static readonly MethodInfo SumWithSingleSelector = GetSumWithSelector<float>();
        internal static readonly MethodInfo SumWithNullableSingleSelector = GetSumWithSelector<float?>();
        internal static readonly MethodInfo SumWithDoubleSelector = GetSumWithSelector<double>();
        internal static readonly MethodInfo SumWithNullableDoubleSelector = GetSumWithSelector<double?>();
        internal static readonly MethodInfo SumWithDecimalSelector = GetSumWithSelector<decimal>();
        internal static readonly MethodInfo SumWithNullableDecimalSelector = GetSumWithSelector<decimal?>();

        internal static readonly MethodInfo SumInt32 = GetSum<int>();
        internal static readonly MethodInfo SumNullableInt32 = GetSum<int?>();
        internal static readonly MethodInfo SumInt64 = GetSum<long>();
        internal static readonly MethodInfo SumNullableInt64 = GetSum<long?>();
        internal static readonly MethodInfo SumSingle = GetSum<float>();
        internal static readonly MethodInfo SumNullableSingle = GetSum<float?>();
        internal static readonly MethodInfo SumDouble = GetSum<double>();
        internal static readonly MethodInfo SumNullableDouble = GetSum<double?>();
        internal static readonly MethodInfo SumDecimal = GetSum<decimal>();
        internal static readonly MethodInfo SumNullableDecimal = GetSum<decimal?>();

        internal static readonly MethodInfo Select = GetQueryableMethod(
            nameof(System.Linq.Queryable.Select),
            [typeof(TSource), typeof(TResult)],
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, TResult>>));

        private static MethodInfo GetAverage<T>() => GetQueryableMethod(
            nameof(System.Linq.Queryable.Average),
            Array.Empty<Type>(),
            typeof(IQueryable<T>));

        private static MethodInfo GetAverageWithSelector<T>() => GetQueryableMethod(
            nameof(System.Linq.Queryable.Average),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, T>>));

        private static MethodInfo GetSum<T>() => GetQueryableMethod(
            nameof(System.Linq.Queryable.Sum),
            Array.Empty<Type>(),
            typeof(IQueryable<T>));

        private static MethodInfo GetSumWithSelector<T>() => GetQueryableMethod(
            nameof(System.Linq.Queryable.Sum),
            typeof(IQueryable<TSource>),
            typeof(Expression<Func<TSource, T>>));

        private static MethodInfo GetQueryableMethod(string name, params Type[] parameterTypes)
            => GetQueryableMethod(name, [typeof(TSource)], parameterTypes);

        private static MethodInfo GetQueryableMethod(string name, Type[] genericArgumentTypes, params Type[] parameterTypes)
            => typeof(System.Linq.Queryable).GetMethodEx(name, genericArgumentTypes, parameterTypes);
    }
}