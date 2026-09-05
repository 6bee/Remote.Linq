// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using Remote.Linq.ExpressionVisitors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_serializing_LambdaExpression_returnung_a_type
{
    public class With_data_contract_serializer() : When_serializing_LambdaExpression_returnung_a_type(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_LambdaExpression_returnung_a_type(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_LambdaExpression_returnung_a_type(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_LoopExpressions(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_LoopExpressions(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_LambdaExpression_returnung_a_type(x => XmlSerializationHelper.CloneExpression(x, [typeof(List<Aqua.TypeSystem.TypeInfo>), typeof(Aqua.TypeSystem.TypeInfo[])]));

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_LambdaExpression_returnung_a_type(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_serializing_LambdaExpression_returnung_a_type(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Func<RemoteLambdaExpression, RemoteLambdaExpression> _serialize;

    protected When_serializing_LambdaExpression_returnung_a_type(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        _serialize = exp => serialize(exp.ReplaceGenericQueryArgumentsByNonGenericArguments());
    }

    [Fact]
    public void Should_support_lambda_returning_typeof_int()
    {
        var type = typeof(int?[]);
        Expression<Func<Type>> transform = () => type;
        var expression = transform.ToRemoteLinqExpression();
        var serialized = _serialize(expression);
        var resurectedExpression = serialized.ToLinqExpression<Func<Type>>();
        resurectedExpression.Compile()().ShouldBe(type);
    }

    [Theory]
    [MemberData(nameof(TestData.TestTypes), MemberType = typeof(TestData))]
    public void Should_support_lambda_returning_type(Type type)
    {
        Expression<Func<Type>> transform = () => type;
        var expression = transform.ToRemoteLinqExpression();
        var serialized = _serialize(expression);
        var resurectedExpression = serialized.ToLinqExpression<Func<Type>>();
        resurectedExpression.Compile()().ShouldBe(type);
    }
}
