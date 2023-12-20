// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_BlockExpression
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter : When_using_BlockExpression
    {
        public With_binary_formatter()
            : base(BinarySerializationHelper.Clone)
        {
        }
    }
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer : When_using_BlockExpression
    {
        public With_data_contract_serializer()
            : base(DataContractSerializationHelper.CloneExpression)
        {
        }
    }

    public class With_newtonsoft_json_serializer : When_using_BlockExpression
    {
        public With_newtonsoft_json_serializer()
            : base(NewtonsoftJsonSerializationHelper.Clone)
        {
        }
    }

    public class With_system_text_json_serializer : When_using_BlockExpression
    {
        public With_system_text_json_serializer()
            : base(SystemTextJsonSerializationHelper.Clone)
        {
        }
    }

#if NETFRAMEWORK
    public class With_net_data_contract_serializer : When_using_BlockExpression
    {
        public With_net_data_contract_serializer()
            : base(NetDataContractSerializationHelper.Clone)
        {
        }
    }
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer : When_using_BlockExpression
    {
        public With_protobuf_net_serializer()
            : base(ProtobufNetSerializationHelper.Clone)
        {
        }
    }

    public class With_xml_serializer : When_using_BlockExpression
    {
        public With_xml_serializer()
            : base(XmlSerializationHelper.CloneExpression)
        {
        }
    }

    private readonly Expression<Func<decimal, string>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_using_BlockExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        ParameterExpression decimalInputParameter = Expression.Parameter(typeof(decimal));

        ParameterExpression formattedDecimalString = Expression.Variable(typeof(string));

        Expression<Func<decimal, string>> expression = Expression.Lambda<Func<decimal, string>>(
            Expression.Block(
                new[] { formattedDecimalString },
                Expression.Assign(
                    decimalInputParameter,
                    Expression.Add(decimalInputParameter, Expression.Constant(1m))),
                Expression.Assign(
                    formattedDecimalString,
                    Expression.Call(
                        decimalInputParameter,
                        typeof(decimal).GetMethod(nameof(decimal.ToString), Type.EmptyTypes))),
                Expression.Call(
                    formattedDecimalString,
                    typeof(string).GetMethod(nameof(string.Replace), new[] { typeof(char), typeof(char) }),
                    Expression.Constant('.'),
                    Expression.Constant(','))),
            decimalInputParameter);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_block_result_should_be_equal()
    {
        var argument = 2.15m;

        string str1 = _originalExpression.Compile()(argument);

        string str2 = _remoteExpression.ToLinqExpression<decimal, string>().Compile()(argument);

        string str3 = _serializedRemoteExpression.ToLinqExpression<decimal, string>().Compile()(argument);

        "3,15"
            .ShouldMatch(str1)
            .ShouldMatch(str2)
            .ShouldMatch(str3);
    }
}