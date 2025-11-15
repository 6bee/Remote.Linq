// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.TypeExtensions;
using System.Collections;
using System.Reflection;

partial class MethodInfos
{
    internal static class Enumerable
    {
        internal static readonly MethodInfo Cast = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.Cast),
            [typeof(TResult)],
            typeof(IEnumerable));

        internal static readonly MethodInfo OfType = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.OfType),
            [typeof(TResult)],
            typeof(IEnumerable));

        internal static readonly MethodInfo ToArray = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.ToArray),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo ToList = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.ToList),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo Contains = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.Contains),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>),
            typeof(TSource));

        internal static readonly MethodInfo Select = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.Select),
            [typeof(TSource), typeof(TResult)],
            typeof(IEnumerable<TSource>),
            typeof(Func<TSource, TResult>));

        internal static readonly MethodInfo Single = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.Single),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo SingleWithPredicate = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.Single),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>),
            typeof(Func<TSource, bool>));

        internal static readonly MethodInfo SingleOrDefault = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.SingleOrDefault),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>));

        internal static readonly MethodInfo SingleOrDefaultWithPredicate = typeof(System.Linq.Enumerable).GetMethodEx(
            nameof(System.Linq.Enumerable.SingleOrDefault),
            [typeof(TSource)],
            typeof(IEnumerable<TSource>),
            typeof(Func<TSource, bool>));
    }
}