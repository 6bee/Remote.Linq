// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Remote.Linq.ExpressionVisitors;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using Xunit;

public static class DataContractSerializationHelper
{
    public static T Clone<T>(T graph)
        => Clone(graph, null);

    public static T Clone<T>(T graph, Type[] knownTypes)
    {
        var serializer = new DataContractSerializer(typeof(T), knownTypes);
        using var stream = new MemoryStream();
        serializer.WriteObject(stream, graph);
        stream.Seek(0, SeekOrigin.Begin);
        return (T)serializer.ReadObject(stream);
    }

    public static T CloneExpression<T>(T expression)
        where T : Remote.Linq.Expressions.Expression
        => CloneExpression(expression, null);

    public static T CloneExpression<T>(T expression, Type[] knownTypes)
        where T : Remote.Linq.Expressions.Expression
    {
        var exp1 = expression.ReplaceGenericQueryArgumentsByNonGenericArguments();
        var exp2 = Clone(exp1, knownTypes);
        var exp3 = exp2.ReplaceNonGenericQueryArgumentsByGenericArguments();
        return exp3;
    }

    public static void SkipUnsupportedDataType(Type type, object value)
    {
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