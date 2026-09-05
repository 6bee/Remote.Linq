// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_serializing_MethodCallExpression
{
    public class With_data_contract_serializer() : When_serializing_MethodCallExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_MethodCallExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_MethodCallExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_MethodCallExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_MethodCallExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_MethodCallExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_MethodCallExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_serializing_MethodCallExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<string, int>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_serializing_MethodCallExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var parameter = Expression.Parameter(typeof(string), "s");

        var expression = Expression.Lambda<Func<string, int>>(
            Expression.Call(
                parameter,
                typeof(string).GetProperty(nameof(string.Length))!.GetGetMethod(),
                Array.Empty<Expression>()),
            parameter);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_return_string_length()
    {
        var input = "hello";

        var result1 = _originalExpression.Compile()(input);
        var result2 = _remoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);

        5
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_zero_for_empty_string()
    {
        var input = string.Empty;

        var result1 = _originalExpression.Compile()(input);
        var result2 = _remoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);

        0
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_return_string_length_for_longer_string()
    {
        var input = "Hello, World!";

        var result1 = _originalExpression.Compile()(input);
        var result2 = _remoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<string, int>>().Compile()(input);

        13
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
