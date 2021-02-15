// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using Aqua.TypeSystem;
    using Shouldly;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class TestHelper
    {
        public static Tuple<T, T> ShouldMatch<T>(this Tuple<T, T> tuple)
        {
            ShouldMatch(tuple.Item1, tuple.Item2);
            return tuple;
        }

        public static T ShouldMatch<T>(this T t1, T t2)
        {
            var isMatch = string.Equals(t1.ToString(), t2.ToString());
            isMatch.ShouldBeTrue($"NO MATCH - {typeof(T)}: \n{t1}\n{t2}");
            return t1;
        }

        public static void ShouldBeSequenceEqual<T>(this IEnumerable<T> t1, IEnumerable<T> t2)
            => t1.SequenceEqual(t2).ShouldBeTrue();

        public static T With<T>(this T t, Action<T> assertion)
        {
            assertion(t);
            return t;
        }

        public static bool TestIs<T>(this object test)
            where T : class
            => test.GetType() == typeof(T);

        public static bool Is<T>(this Type type)
            where T : struct
            => type == typeof(T)
            || type == typeof(T?)
            || typeof(ICollection<T>).IsAssignableFrom(type)
            || typeof(ICollection<T?>).IsAssignableFrom(type);

        public static bool IsNotPublic(this Type type)
            => type.IsNotPublic
            || TypeHelper.GetElementType(type).IsNotPublic;

        public static bool IsEnum(this Type type)
        {
            if (type.IsEnum)
            {
                return true;
            }

            var elementType = TypeHelper.GetElementType(type);
            return (type.IsCollection() && elementType.IsEnum)
                || (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition() && type.GetGenericArguments()[0].IsEnum)
                || (type.IsCollection() && elementType.IsGenericType && typeof(Nullable<>) == elementType.GetGenericTypeDefinition() && elementType.GetGenericArguments()[0].IsEnum);
        }

        public static bool IsCollection(this Type type)
            => typeof(IEnumerable).IsAssignableFrom(type)
            && type != typeof(string);

        public static IDisposable CreateCultureContext(string culture) => CultureInfo.GetCultureInfo(culture).CreateContext();

        public static IDisposable CreateContext(this CultureInfo culture) => new CultureContext(culture);

        private sealed class CultureContext : IDisposable
        {
            private readonly CultureInfo _culture;

            public CultureContext(CultureInfo culture)
            {
                _culture = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = culture;
            }

            public void Dispose()
            {
                CultureInfo.CurrentCulture = _culture;
            }
        }

        public static Type AsNonNullableType(this Type type)
        {
            var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return isNullable ? type.GetGenericArguments()[0] : type;
        }

        public static void ShouldBeNullable(this Type type)
        {
            var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (isNullable)
            {
                return;
            }

            throw new Xunit.Sdk.IsNotTypeException(typeof(Nullable<>), type);
        }
    }
}