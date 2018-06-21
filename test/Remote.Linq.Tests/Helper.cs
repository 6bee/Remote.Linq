// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using Shouldly;
    using System;

    public static class Helper
    {
        public static Tuple<T, T> ShouldMatch<T>(this Tuple<T, T> tuple)
        {
            ShouldMatch(tuple.Item1, tuple.Item2);
            return tuple;
        }

        public static T ShouldMatch<T>(this T t1, T t2)
        {
            var isMatch = string.Equals(t1.ToString(), t2.ToString());
            isMatch.ShouldBeTrue(() => $"NO MATCH - {typeof(T)}: \n{t1}\n{t2}");
            return t1;
        }

        public static T With<T>(this T t, Action<T> assertion)
        {
            assertion(t);
            return t;
        }

        public static bool TestIs<T>(this object test) where T : class
            => test.GetType() == typeof(T);

        public static bool Is<T>(this Type type) where T : struct
            => type == typeof(T)
            || type == typeof(T?)
            || type == typeof(T[])
            || type == typeof(T?[]);

        public static bool NetCoreApp1_0
#if NETCOREAPP1_0
            => true;
#else
            => false;
#endif
    }
}