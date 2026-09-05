// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Linq.Expressions;
using Xunit;
using RemoteExpression = Remote.Linq.Expressions.Expression;

public abstract class When_serializing_DefaultExpression
{
    public class With_data_contract_serializer() : When_serializing_DefaultExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_DefaultExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_DefaultExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_DefaultExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_DefaultExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_DefaultExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_DefaultExpression(BinarySerializationHelper.CloneExpression);

    public class With_net_data_contract_serializer() : When_serializing_DefaultExpression(NetDataContractSerializationHelper.CloneExpression);
#endif // NETFRAMEWORK

    private readonly Expression _originalExpression;

    private readonly RemoteExpression _remoteExpression;

    private readonly RemoteExpression _serializedRemoteExpression;

    protected When_serializing_DefaultExpression(Func<RemoteExpression, RemoteExpression> serialize)
    {
        var expression = Expression.Default(typeof(int));

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_default_int()
    {
        int result1 = Expression.Lambda<Func<int>>(_originalExpression).Compile()();

        int result2 = Expression.Lambda<Func<int>>(_remoteExpression.ToLinqExpression()).Compile()();

        int result3 = Expression.Lambda<Func<int>>(_serializedRemoteExpression.ToLinqExpression()).Compile()();

        0
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
