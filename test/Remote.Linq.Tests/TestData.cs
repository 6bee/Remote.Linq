// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using Aqua.Dynamic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;

    public static class TestData
    {
        public enum Custom
        {
            Foo,
            Bar,
        }

        public static IEnumerable<object[]> Types => new Type[]
            {
                typeof(byte),
                typeof(int),
                typeof(ulong),
                typeof(string),
                typeof(DateTime),
                typeof(Custom),
                new { Text = string.Empty, Timestamp = default(DateTime?) }.GetType(),
            }
            .SelectMany(x => new[]
            {
                x,
                x.IsClass ? x : typeof(Nullable<>).MakeGenericType(x),
            })
            .SelectMany(x => new[]
            {
                x,
                typeof(List<>).MakeGenericType(x),
                x.MakeArrayType(),
            })
            .Distinct()
            .Select(x => new[] { x });

        public static IEnumerable<object[]> PrimitiveValues => new object[]
            {
                $"Test values treated as native types in {nameof(DynamicObjectMapper)}",
                byte.MinValue,
                byte.MaxValue,
                sbyte.MinValue,
                sbyte.MaxValue,
                short.MinValue,
                short.MaxValue,
                ushort.MinValue,
                ushort.MaxValue,
                int.MinValue,
                int.MaxValue,
                uint.MinValue,
                uint.MaxValue,
                long.MinValue,
                long.MaxValue,
                ulong.MinValue,
                ulong.MaxValue,
                float.MinValue,
                float.MaxValue,
                double.MinValue,
                double.MaxValue,
                decimal.MinValue,
                decimal.MaxValue,
                new decimal(Math.E),
                new decimal(Math.PI),
                char.MinValue,
                char.MaxValue,
                'à',
                true,
                false,
                default(Guid),
                Guid.NewGuid(),
                default(DateTime),
                DateTime.Now,
                Custom.Foo,
                Custom.Bar,
                default(TimeSpan),
                new TimeSpan(long.MaxValue),
                default(DateTimeOffset),
                DateTimeOffset.MinValue,
                DateTimeOffset.MaxValue,
                new DateTimeOffset(new DateTime(2012, 12, 12), new TimeSpan(12, 12, 0)),
                default(BigInteger),
                new BigInteger(ulong.MinValue) - 1,
                new BigInteger(ulong.MaxValue) + 1,
                default(Complex),
                new Complex(32, -87654),
                new Complex(-87654, 234),
                new Complex(double.MinValue, double.MinValue),
                new Complex(double.MaxValue, double.MaxValue),
#if NET5_0
                (Half)Math.PI,
                Half.MinValue,
                Half.MaxValue,
#endif // NET5_0
            }
            .SelectMany(x => new Tuple<Type, object>[]
            {
                Tuple.Create(x.GetType(), x),
                Tuple.Create(x.GetType().IsClass ? x.GetType() : typeof(Nullable<>).MakeGenericType(x.GetType()), x),
                Tuple.Create(x.GetType().IsClass ? x.GetType() : typeof(Nullable<>).MakeGenericType(x.GetType()), default(object)),
            })
            .Distinct()
            .Select(x => new object[] { x.Item1, x.Item2 });

        public static IEnumerable<object[]> PrimitiveValueArrays => PrimitiveValues
            .Select(x => new[]
            {
                ((Type)x[0]).MakeArrayType(),
                CreateArray((Type)x[0], x[1]),
            });

        public static IEnumerable<object[]> PrimitiveValueLists => PrimitiveValues
            .Select(x => new[]
            {
                typeof(List<>).MakeGenericType((Type)x[0]),
                CreateList((Type)x[0], x[1]),
            });

        private static object CreateArray(Type type, object item)
        {
            var toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
            return toArrayMethod.Invoke(null, new[] { CreateEnumerable(type, item) });
        }

        private static object CreateList(Type type, object item)
        {
            var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
            return toListMethod.Invoke(null, new[] { CreateEnumerable(type, item) });
        }

        private static object CreateEnumerable(Type type, object item)
        {
            var array = new[] { item, item };
            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
            return castMethod.Invoke(null, new[] { array });
        }
    }
}
