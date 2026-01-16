// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument;

using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using Xunit;

public abstract class When_using_local_variable_query_string_argument_list
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter() : When_using_local_variable_query_string_argument_list(BinarySerializationHelper.Clone);
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer() : When_using_local_variable_query_string_argument_list(x => DataContractSerializationHelper.CloneExpression(x, [typeof(List<string>)]));

    public class With_newtonsoft_json_serializer() : When_using_local_variable_query_string_argument_list(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_local_variable_query_string_argument_list(SystemTextJsonSerializationHelper.Clone);

#if NETFRAMEWORK
    public class With_net_data_contract_serializer() : When_using_local_variable_query_string_argument_list(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer() : When_using_local_variable_query_string_argument_list(ProtobufNetSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_local_variable_query_string_argument_list(x => XmlSerializationHelper.CloneExpression(x, [typeof(List<string>)]));

    private class AType
    {
        public string Key { get; set; }
    }

    private readonly LambdaExpression _remoteExpression;

    private readonly LambdaExpression _serializedRemoteExpression;

    protected When_using_local_variable_query_string_argument_list(Func<LambdaExpression, LambdaExpression> serialize)
    {
        var keys = new List<string> { "K1", "K2" };

        System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => keys.Contains(x.Key);

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
        var exp1 = _remoteExpression.ToLinqExpression<AType, bool>();
        var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, bool>();

        exp1.ShouldEqualExpression(exp2);
    }
}