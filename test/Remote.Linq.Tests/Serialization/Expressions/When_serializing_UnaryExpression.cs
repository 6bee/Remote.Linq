// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_serializing_UnaryExpression
{
    public class With_data_contract_serializer() : When_serializing_UnaryExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_UnaryExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_UnaryExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_UnaryExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_UnaryExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_UnaryExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_UnaryExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_serializing_UnaryExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<bool, bool>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_serializing_UnaryExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var x = Expression.Parameter(typeof(bool), "x");
        var expression = Expression.Lambda<Func<bool, bool>>(
            Expression.Not(x),
            x);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_true_when_input_is_false()
    {
        var input = false;

        bool result1 = _originalExpression.Compile()(input);
        bool result2 = _remoteExpression.ToLinqExpression<Func<bool, bool>>().Compile()(input);
        bool result3 = _serializedRemoteExpression.ToLinqExpression<Func<bool, bool>>().Compile()(input);

        true
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_false_when_input_is_true()
    {
        var input = true;

        bool result1 = _originalExpression.Compile()(input);
        bool result2 = _remoteExpression.ToLinqExpression<Func<bool, bool>>().Compile()(input);
        bool result3 = _serializedRemoteExpression.ToLinqExpression<Func<bool, bool>>().Compile()(input);

        false
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
