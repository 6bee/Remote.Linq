// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using Remote.Linq.Expressions;
using System;
using Xunit;

public abstract class When_using_complex_expression_with_local_lambda
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter() : When_using_complex_expression_with_local_lambda(BinarySerializationHelper.Clone);
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer() : When_using_complex_expression_with_local_lambda(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_complex_expression_with_local_lambda(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_complex_expression_with_local_lambda(SystemTextJsonSerializationHelper.Clone);

#if NETFRAMEWORK
    public class With_net_data_contract_serializer() : When_using_complex_expression_with_local_lambda(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer() : When_using_complex_expression_with_local_lambda(ProtobufNetSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_complex_expression_with_local_lambda(XmlSerializationHelper.CloneExpression);

    private readonly LambdaExpression _remoteExpression;

    private readonly LambdaExpression _serializedRemoteExpression;

    protected When_using_complex_expression_with_local_lambda(Func<LambdaExpression, LambdaExpression> serialize)
    {
        Func<object, string> sufix = (x) => x + "ending";

        System.Linq.Expressions.Expression<Func<bool, bool>> expression = x => sufix("test").Length > 10;

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
        var exp1 = _remoteExpression.ToLinqExpression<bool, bool>();
        var exp2 = _serializedRemoteExpression.ToLinqExpression<bool, bool>();

        exp1.ShouldEqualExpression(exp2);
    }
}