// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using Remote.Linq.ExpressionVisitors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_LambdaExpression_returnung_a_type
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter : When_using_LambdaExpression_returnung_a_type
    {
        public With_binary_formatter()
            : base(BinarySerializationHelper.Clone)
        {
        }
    }
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer : When_using_LambdaExpression_returnung_a_type
    {
        public With_data_contract_serializer()
            : base(DataContractSerializationHelper.CloneExpression)
        {
        }
    }

    public class With_newtonsoft_json_serializer : When_using_LambdaExpression_returnung_a_type
    {
        public With_newtonsoft_json_serializer()
            : base(NewtonsoftJsonSerializationHelper.Clone)
        {
        }
    }

    public class With_system_text_json_serializer : When_using_LambdaExpression_returnung_a_type
    {
        public With_system_text_json_serializer()
            : base(SystemTextJsonSerializationHelper.Clone)
        {
        }
    }

#if NETFRAMEWORK
    public class With_net_data_contract_serializer : When_using_LambdaExpression_returnung_a_type
    {
        public With_net_data_contract_serializer()
            : base(NetDataContractSerializationHelper.Clone)
        {
        }
    }
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer : When_using_LoopExpressions
    {
        public With_protobuf_net_serializer()
            : base(ProtobufNetSerializationHelper.Clone)
        {
        }
    }

    public class With_xml_serializer : When_using_LambdaExpression_returnung_a_type
    {
        public With_xml_serializer()
            : base(x => XmlSerializationHelper.CloneExpression(x, [typeof(List<Aqua.TypeSystem.TypeInfo>), typeof(Aqua.TypeSystem.TypeInfo[])]))
        {
        }
    }

    private readonly Func<RemoteLambdaExpression, RemoteLambdaExpression> _serialize;

    protected When_using_LambdaExpression_returnung_a_type(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
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
        var resurectedExpression = serialized.ToLinqExpression<Type>();
        resurectedExpression.Compile()().ShouldBe(type);
    }

    [Theory]
    [MemberData(nameof(TestData.TestTypes), MemberType = typeof(TestData))]
    public void Should_support_lambda_returning_type(Type type)
    {
        Expression<Func<Type>> transform = () => type;
        var expression = transform.ToRemoteLinqExpression();
        var serialized = _serialize(expression);
        var resurectedExpression = serialized.ToLinqExpression<Type>();
        resurectedExpression.Compile()().ShouldBe(type);
    }
}