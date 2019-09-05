// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class MethodInfos
    {
        private class ProbingType
        {
            private ProbingType()
            {
            }
        }

        private static MethodInfo GetStaticMethod(Type declaringType, string name, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public)
            => GetStaticMethod(declaringType, name, x => true, bindingFlags);

        private static MethodInfo GetStaticMethod(Type declaringType, string name, int parameterCount, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public)
            => GetStaticMethod(declaringType, name, x => x.GetParameters().Length == parameterCount, bindingFlags);

        private static MethodInfo GetStaticMethod(Type declaringType, string name, Func<MethodInfo, bool> filter, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public)
            => declaringType
                .GetMethods(bindingFlags)
                .Single(x => string.Equals(x.Name, name, StringComparison.Ordinal) && filter(x));

        internal static class Enumerable
        {
            internal static readonly MethodInfo Cast = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Cast));
            internal static readonly MethodInfo OfType = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.OfType));
            internal static readonly MethodInfo ToArray = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.ToArray));
            internal static readonly MethodInfo ToList = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.ToList));
            internal static readonly MethodInfo Contains = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Contains), 2);
            internal static readonly MethodInfo Single = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Single), 1);
            internal static readonly MethodInfo SingleWithPredicate = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Single), 2);
            internal static readonly MethodInfo SingleOrDefault = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.SingleOrDefault), 1);
            internal static readonly MethodInfo SingleOrDefaultWithPredicate = GetStaticMethod(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.SingleOrDefault), 2);
        }

        internal static class Expression
        {
            internal static readonly MethodInfo Lambda = GetStaticMethod(
                typeof(Expression),
                nameof(Expression.Lambda),
                m =>
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));
        }

        internal static class Queryable
        {
            internal static readonly MethodInfo OrderBy = GetGenericQueryableMethod(nameof(System.Linq.Queryable.OrderBy), 2);
            internal static readonly MethodInfo OrderByDescending = GetGenericQueryableMethod(nameof(System.Linq.Queryable.OrderByDescending), 2);
            internal static readonly MethodInfo ThenBy = GetGenericQueryableMethod(nameof(System.Linq.Queryable.ThenBy), 2);
            internal static readonly MethodInfo ThenByDescending = GetGenericQueryableMethod(nameof(System.Linq.Queryable.ThenByDescending), 2);
            internal static readonly MethodInfo Aggregate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Aggregate), 2);
            internal static readonly MethodInfo AggregateWithSeed = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Aggregate), 3);
            internal static readonly MethodInfo AggregateWithSeedAndSelector = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Aggregate), 4);
            internal static readonly MethodInfo All = GetGenericQueryableMethod(nameof(System.Linq.Queryable.All), 2);
            internal static readonly MethodInfo Any = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Any), 1);
            internal static readonly MethodInfo AnyWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Any), 2);
            internal static readonly MethodInfo AverageDouble = GetAverage<double>();
            internal static readonly MethodInfo AverageInt32 = GetAverage<int>();
            internal static readonly MethodInfo AverageNullableInt32 = GetAverage<int?>();
            internal static readonly MethodInfo AverageInt64 = GetAverage<long>();
            internal static readonly MethodInfo AverageNullableInt64 = GetAverage<long?>();
            internal static readonly MethodInfo AverageSingle = GetAverage<float>();
            internal static readonly MethodInfo AverageNullableSingle = GetAverage<float?>();
            internal static readonly MethodInfo AverageNullableDouble = GetAverage<double?>();
            internal static readonly MethodInfo AverageDecimal = GetAverage<decimal>();
            internal static readonly MethodInfo AverageNullableDecimal = GetAverage<decimal?>();
            internal static readonly MethodInfo AverageWithInt32Selector = GetAverageWithSelector<int>();
            internal static readonly MethodInfo AverageWithNullableInt32Selector = GetAverageWithSelector<int?>();
            internal static readonly MethodInfo AverageWithSingleSelector = GetAverageWithSelector<float>();
            internal static readonly MethodInfo AverageWithNullableSingleSelector = GetAverageWithSelector<float?>();
            internal static readonly MethodInfo AverageWithInt64Selector = GetAverageWithSelector<long>();
            internal static readonly MethodInfo AverageWithNullableInt64Selector = GetAverageWithSelector<long?>();
            internal static readonly MethodInfo AverageWithDoubleSelector = GetAverageWithSelector<double>();
            internal static readonly MethodInfo AverageWithNullableDoubleSelector = GetAverageWithSelector<double?>();
            internal static readonly MethodInfo AverageWithDecimalSelector = GetAverageWithSelector<decimal>();
            internal static readonly MethodInfo AverageWithNullableDecimalSelector = GetAverageWithSelector<decimal?>();
            internal static readonly MethodInfo ContainsWithComparer = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Contains), 3);
            internal static readonly MethodInfo Contains = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Contains), 2);
            internal static readonly MethodInfo CountWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Count), 2);
            internal static readonly MethodInfo Count = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Count), 1);
            internal static readonly MethodInfo ElementAt = GetGenericQueryableMethod(nameof(System.Linq.Queryable.ElementAt), 2);
            internal static readonly MethodInfo ElementAtOrDefault = GetGenericQueryableMethod(nameof(System.Linq.Queryable.ElementAtOrDefault), 2);
            internal static readonly MethodInfo First = GetGenericQueryableMethod(nameof(System.Linq.Queryable.First), 1);
            internal static readonly MethodInfo FirstWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.First), 2);
            internal static readonly MethodInfo FirstOrDefaultWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.FirstOrDefault), 2);
            internal static readonly MethodInfo FirstOrDefault = GetGenericQueryableMethod(nameof(System.Linq.Queryable.FirstOrDefault), 1);
            internal static readonly MethodInfo Last = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Last), 1);
            internal static readonly MethodInfo LastWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Last), 2);
            internal static readonly MethodInfo LastOrDefault = GetGenericQueryableMethod(nameof(System.Linq.Queryable.LastOrDefault), 1);
            internal static readonly MethodInfo LastOrDefaultWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.LastOrDefault), 2);
            internal static readonly MethodInfo LongCountWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.LongCount), 2);
            internal static readonly MethodInfo LongCount = GetGenericQueryableMethod(nameof(System.Linq.Queryable.LongCount), 1);
            internal static readonly MethodInfo MaxWithSelector = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Max), 2);
            internal static readonly MethodInfo Max = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Max), 1);
            internal static readonly MethodInfo MinWithSelector = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Min), 2);
            internal static readonly MethodInfo Min = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Min), 1);
            internal static readonly MethodInfo SequenceEqualWithComparer = GetGenericQueryableMethod(nameof(System.Linq.Queryable.SequenceEqual), 3);
            internal static readonly MethodInfo SequenceEqual = GetGenericQueryableMethod(nameof(System.Linq.Queryable.SequenceEqual), 2);
            internal static readonly MethodInfo SingleWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Single), 2);
            internal static readonly MethodInfo Single = GetGenericQueryableMethod(nameof(System.Linq.Queryable.Single), 1);
            internal static readonly MethodInfo SingleOrDefault = GetGenericQueryableMethod(nameof(System.Linq.Queryable.SingleOrDefault), 1);
            internal static readonly MethodInfo SingleOrDefaultWithPredicate = GetGenericQueryableMethod(nameof(System.Linq.Queryable.SingleOrDefault), 2);
            internal static readonly MethodInfo SumWithDecimalSelector = GetSumWithSelector<decimal>();
            internal static readonly MethodInfo SumWithNullableDecimalSelector = GetSumWithSelector<decimal?>();
            internal static readonly MethodInfo SumWithNullableDoubleSelector = GetSumWithSelector<double?>();
            internal static readonly MethodInfo SumWithDoubleSelector = GetSumWithSelector<double>();
            internal static readonly MethodInfo SumNullableInt32 = GetSum<int?>();
            internal static readonly MethodInfo SumWithSingleSelector = GetSumWithSelector<float>();
            internal static readonly MethodInfo SumInt32 = GetSum<int>();
            internal static readonly MethodInfo SumInt64 = GetSum<long>();
            internal static readonly MethodInfo SumNullableInt64 = GetSum<long?>();
            internal static readonly MethodInfo SumSingle = GetSum<float>();
            internal static readonly MethodInfo SumNullableSingle = GetSum<float?>();
            internal static readonly MethodInfo SumDouble = GetSum<double>();
            internal static readonly MethodInfo SumNullableDouble = GetSum<double?>();
            internal static readonly MethodInfo SumWithNullableSingleSelector = GetSumWithSelector<float?>();
            internal static readonly MethodInfo SumDecimal = GetSum<decimal>();
            internal static readonly MethodInfo SumNullableDecimal = GetSum<decimal?>();
            internal static readonly MethodInfo SumWithInt32Selector = GetSumWithSelector<int>();
            internal static readonly MethodInfo SumWithNullableInt64Selector = GetSumWithSelector<long?>();
            internal static readonly MethodInfo SumWithInt64Selector = GetSumWithSelector<long>();
            internal static readonly MethodInfo SumWithNullableInt32Selector = GetSumWithSelector<int?>();

            private static MethodInfo GetQueryableMethod(string name, Func<MethodInfo, bool> filter)
                => GetStaticMethod(typeof(System.Linq.Queryable), name, filter);

            private static MethodInfo GetGenericQueryableMethod(string name, int parameterCount)
                => GetStaticMethod(typeof(System.Linq.Queryable), name, m => m.IsGenericMethod && m.GetParameters().Length == parameterCount);

            internal static readonly MethodInfo Select = GetQueryableMethod(
                nameof(System.Linq.Queryable.Select),
                m =>
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2 &&
                    m.MakeGenericMethod(typeof(ProbingType), typeof(ProbingType)).GetParameters().Last().ParameterType == typeof(Expression<Func<ProbingType, ProbingType>>));

            private static MethodInfo GetAverage<T>() => GetQueryableMethod(
                nameof(System.Linq.Queryable.Average),
                m => !m.IsGenericMethod && m.GetParameters().First().ParameterType == typeof(IQueryable<T>));

            private static MethodInfo GetAverageWithSelector<T>() => GetQueryableMethod(
                nameof(System.Linq.Queryable.Average),
                m =>
                    m.IsGenericMethod &&
                    m.MakeGenericMethod(typeof(ProbingType)).GetParameters().Last().ParameterType == typeof(Expression<Func<ProbingType, T>>));

            private static MethodInfo GetSum<T>() => GetQueryableMethod(
                nameof(System.Linq.Queryable.Sum),
                m => !m.IsGenericMethod && m.GetParameters().First().ParameterType == typeof(IQueryable<T>));

            private static MethodInfo GetSumWithSelector<T>() => GetQueryableMethod(
                nameof(System.Linq.Queryable.Sum),
                m =>
                    m.IsGenericMethod &&
                    m.MakeGenericMethod(typeof(ProbingType)).GetParameters().Last().ParameterType == typeof(Expression<Func<ProbingType, T>>));
        }

        internal static class String
        {
            internal static readonly MethodInfo StartsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
            internal static readonly MethodInfo EndsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
            internal static readonly MethodInfo Contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        }

        internal static class QueryFuntion
        {
            internal static readonly MethodInfo Include = GetStaticMethod(
                typeof(QueryFunctions),
                nameof(QueryFunctions.Include),
                x =>
                    x.IsGenericMethod &&
                    x.GetParameters().Length == 2 &&
                    x.GetParameters()[1].ParameterType == typeof(string));
        }

        internal static class GroupingFactory
        {
            internal static readonly MethodInfo MapMany = GetStaticMethod(typeof(GroupingFactory), nameof(InternalMapMany), BindingFlags.Static | BindingFlags.NonPublic);
            internal static readonly MethodInfo MapOne = GetStaticMethod(typeof(GroupingFactory), nameof(InternalMapOne), BindingFlags.Static | BindingFlags.NonPublic);

            private static IEnumerable<Grouping<TKey, TElement>> InternalMapMany<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> list)
                => list.Select(InternalMapOne).ToArray();

            private static Grouping<TKey, TElement> InternalMapOne<TKey, TElement>(IGrouping<TKey, TElement> grouping)
                => new Grouping<TKey, TElement> { Key = grouping.Key, Elements = grouping.ToArray() };
        }
    }
}
