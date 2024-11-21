// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests;

using Aqua.Dynamic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;

public static class TestData
{
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

    public enum Custom
    {
        Foo,
        Bar,
    }

    private static IEnumerable<(Type Type, object Value)> CreateTestValues()
        => new object[]
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
#if NET8_0_OR_GREATER
            (Half)Math.PI,
            Half.MinValue,
            Half.MaxValue,
#endif // NET8_0_OR_GREATER
            new { Text = string.Empty, Timestamp = default(DateTime?) },
        }
        .SelectMany(x => new (Type Type, object Value)[]
        {
            (x.GetType(), x),
            (x.GetType(), CreateDefault(x.GetType())),
            (x.GetType().IsClass ? x.GetType() : typeof(Nullable<>).MakeGenericType(x.GetType()), x),
            (x.GetType().IsClass ? x.GetType() : typeof(Nullable<>).MakeGenericType(x.GetType()), default(object)),
        })
        .Distinct();

    private static object CreateDefault(Type t)
        => typeof(TestData).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
        .Single(x => string.Equals(x.Name, nameof(CreateDefault), StringComparison.Ordinal) && x.IsGenericMethodDefinition)
        .MakeGenericMethod(t)
        .Invoke(null, null);

    private static object CreateDefault<T>()
        => default(T);

    private static IEnumerable<string> CreateTestCultureNames()
        => new[]
        {
            string.Empty,
            "da-DK",
            "de-DE",
            "en-US",
            "tzm-Arab-MA",
            "zh-CHS",
        };

    public static IEnumerable<object[]> TestCultureNames
        => CreateTestCultureNames()
        .Select(x => new[] { x });

    public static IEnumerable<object[]> TestCultures
        => CreateTestCultureNames()
        .Select(CultureInfo.GetCultureInfo)
        .Select(x => new[] { x });

    public static IEnumerable<object[]> TestTypes
        => CreateTestValues()
        .Select(x => x.Type)
        .Distinct()
        .Select(x => new[] { x });

    public static IEnumerable<object[]> TestValues
        => CreateTestValues()
        .Select(x => new object[] { x.Type, x.Value });

    public static IEnumerable<object[]> TestValueArrays
        => CreateTestValues()
        .Select(x => new[]
        {
            x.Type.MakeArrayType(),
            CreateArray(x.Type, x.Value),
        });

    public static IEnumerable<object[]> TestValueLists
        => CreateTestValues()
        .Select(x => new[]
        {
            typeof(List<>).MakeGenericType(x.Type),
            CreateList(x.Type, x.Value),
        });

    private static object CreateArray(Type type, object item)
    {
        var toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray), PublicStatic).MakeGenericMethod(type);
        return toArrayMethod.Invoke(null, [CreateEnumerable(type, item)]);
    }

    private static object CreateList(Type type, object item)
    {
        var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList), PublicStatic).MakeGenericMethod(type);
        return toListMethod.Invoke(null, [CreateEnumerable(type, item)]);
    }

    private static object CreateEnumerable(Type type, object item)
    {
        var array = new[] { item, item };
        var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast), PublicStatic).MakeGenericMethod(type);
        return castMethod.Invoke(null, new[] { array });
    }
}