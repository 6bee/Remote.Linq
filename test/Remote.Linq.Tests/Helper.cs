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
    }
}