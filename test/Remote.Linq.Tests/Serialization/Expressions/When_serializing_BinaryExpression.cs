// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_serializing_BinaryExpression
{
    public class With_data_contract_serializer() : When_serializing_BinaryExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_BinaryExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_BinaryExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_BinaryExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_BinaryExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_BinaryExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_BinaryExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_serializing_BinaryExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<int, int, bool>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_serializing_BinaryExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var a = Expression.Parameter(typeof(int), "a");
        var b = Expression.Parameter(typeof(int), "b");
        var expression = Expression.Lambda<Func<int, int, bool>>(Expression.GreaterThan(a, b), a, b);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_true_when_first_is_greater()
    {
        var result1 = _originalExpression.Compile()(5, 3);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(5, 3);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(5, 3);

        true
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_false_when_first_is_less()
    {
        var result1 = _originalExpression.Compile()(3, 5);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(3, 5);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(3, 5);

        false
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_false_when_equal()
    {
        var result1 = _originalExpression.Compile()(5, 5);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(5, 5);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, bool>>().Compile()(5, 5);

        false
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
