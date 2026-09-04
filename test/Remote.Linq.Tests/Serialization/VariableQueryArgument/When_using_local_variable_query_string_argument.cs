// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument;

using Remote.Linq.Expressions;
using System;
using Xunit;

public abstract class When_using_local_variable_query_string_argument
{
    public class With_data_contract_serializer() : When_using_local_variable_query_string_argument(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_local_variable_query_string_argument(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_local_variable_query_string_argument(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_local_variable_query_string_argument(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_local_variable_query_string_argument(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_local_variable_query_string_argument(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_local_variable_query_string_argument(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_local_variable_query_string_argument(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private class AType
    {
        public string Key { get; set; }
    }

    private readonly LambdaExpression _remoteExpression;

    private readonly LambdaExpression _serializedRemoteExpression;

    protected When_using_local_variable_query_string_argument(Func<LambdaExpression, LambdaExpression> serialize)
    {
        var key = "K1";

        System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => x.Key == key;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Remote_expression_should_be_equal()
    {
        _remoteExpression.ShouldEqualRemoteExpression(_serializedRemoteExpression);
    }

    [Fact]
    public void System_expresison_should_be_equal()
    {
        var exp1 = _remoteExpression.ToLinqExpression<Func<AType, bool>>();
        var exp2 = _serializedRemoteExpression.ToLinqExpression<Func<AType, bool>>();

        exp1.ShouldEqualExpression(exp2);
    }
}
