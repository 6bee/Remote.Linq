// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_InvokeExpression
{
    public class With_data_contract_serializer() : When_using_InvokeExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_InvokeExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_InvokeExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_InvokeExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_InvokeExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_InvokeExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_InvokeExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_InvokeExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<decimal, bool>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    protected When_using_InvokeExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        Expression<Func<decimal, bool>> exp = x => x <= 0m;

        var parameter = Expression.Parameter(typeof(decimal), "x");

        var expression = Expression.Lambda<Func<decimal, bool>>(
            Expression.Invoke(exp, parameter), parameter);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_block_result_should_be_equal()
    {
        var argument = 0.00001m;

        bool result1 = _originalExpression.Compile()(argument);

        bool result2 = _remoteExpression.ToLinqExpression<Func<decimal, bool>>().Compile()(argument);

        bool result3 = _serializedRemoteExpression.ToLinqExpression<Func<decimal, bool>>().Compile()(argument);

        false
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
