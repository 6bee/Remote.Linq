// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Linq.Expressions;
using Xunit;
using RemoteConstantExpression = Remote.Linq.Expressions.ConstantExpression;
using RemoteExpression = Remote.Linq.Expressions.Expression;

public abstract class When_using_ConstantExpression
{
    public class With_data_contract_serializer() : When_using_ConstantExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_ConstantExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_ConstantExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_ConstantExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_ConstantExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_ConstantExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_ConstantExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_ConstantExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly ConstantExpression _originalConstant;

    private readonly RemoteExpression _remoteConstant;

    private readonly RemoteExpression _serializedRemoteConstant;

    protected When_using_ConstantExpression(Func<RemoteExpression, RemoteExpression> serialize)
    {
        _originalConstant = Expression.Constant(42);

        _remoteConstant = _originalConstant.ToRemoteLinqExpression();

        _serializedRemoteConstant = serialize(_remoteConstant);
    }

    [Fact]
    public void Expression_value_should_be_equal()
    {
        var value1 = ((ConstantExpression)_originalConstant).Value;
        var value2 = ((ConstantExpression)_remoteConstant.ToLinqExpression()).Value;
        var value3 = ((ConstantExpression)_serializedRemoteConstant.ToLinqExpression()).Value;

        42
            .ShouldMatch(value1)
            .ShouldMatch(value2)
            .ShouldMatch(value3);
    }

    [Fact]
    public void Expression_result_should_be_equal()
    {
        int result1 = Expression.Lambda<Func<int>>(_originalConstant).Compile()();
        int result2 = Expression.Lambda<Func<int>>(_remoteConstant.ToLinqExpression()).Compile()();
        int result3 = Expression.Lambda<Func<int>>(_serializedRemoteConstant.ToLinqExpression()).Compile()();

        42
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
