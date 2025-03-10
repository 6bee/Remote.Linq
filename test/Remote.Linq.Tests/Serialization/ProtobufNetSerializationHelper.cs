﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.ProtoBuf;
using System;
using System.Linq;
using System.Numerics;
using Xunit;
using TypeModel = global::ProtoBuf.Meta.TypeModel;

public static class ProtobufNetSerializationHelper
{
    private static readonly TypeModel _configuration = CreateTypeModel();

    private static TypeModel CreateTypeModel()
    {
        var configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

        var testdatatypes = TestData.TestTypes
            .Select(x => (Type)x[0])
            .Select(x => x.AsNonNullableType())
            .Distinct()
            .ToArray();
        testdatatypes.ForEach(x => configuration.AddDynamicPropertyType(x));

        return configuration;
    }

    public static T Clone<T>(this T graph) => Clone(graph, null);

    public static T Clone<T>(this T graph, TypeModel configuration)
    {
        configuration ??= _configuration;
        return (T)configuration.DeepClone(graph);
    }

    public static TypeModel CreateModelFor(Type type)
    {
        if (type.IsCollection())
        {
            type = TypeHelper.GetElementType(type);
        }

        if (type.IsEnum)
        {
            type = typeof(string);
        }

        return ProtoBufTypeModel.ConfigureRemoteLinq()
            .AddDynamicPropertyType(type)
            .Compile();
    }

    public static void SkipUnsupportedDataType(Type type, object value)
    {
        Skip.If(type.Is<DateTimeOffset>(), $"{type} not supported by out-of-the-box protobuf-net");
        Skip.If(type.Is<BigInteger>(), $"{type} not supported by out-of-the-box protobuf-net");
        Skip.If(type.Is<Complex>(), $"{type} not supported by out-of-the-box protobuf-net");
        Skip.If(type.IsNotPublic(), $"Not-public {type} not supported protobuf-net");
#if NET5_0_OR_GREATER
        Skip.If(type.Is<Half>(), $"{type} serialization is not supported.");
#endif // NET5_0_OR_GREATER
#if NET6_0_OR_GREATER
        Skip.If(type.Is<DateOnly>(), $"{type} serialization is not supported.");
        Skip.If(type.Is<TimeOnly>(), $"{type} serialization is not supported.");
#endif // NET6_0_OR_GREATER
#if NET7_0_OR_GREATER
        Skip.If(type.Is<Int128>(), $"{type} serialization is not supported.");
        Skip.If(type.Is<UInt128>(), $"{type} serialization is not supported.");
#endif // NET7_0_OR_GREATER
    }
}