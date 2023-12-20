// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Remote.Linq.ExpressionVisitors;
using System;
using System.IO;
using System.Runtime.Serialization;

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
}