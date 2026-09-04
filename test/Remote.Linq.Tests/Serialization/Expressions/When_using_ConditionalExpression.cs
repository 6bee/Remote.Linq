// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_ConditionalExpression
{
    public class With_data_contract_serializer() : When_using_ConditionalExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_ConditionalExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_ConditionalExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_ConditionalExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_ConditionalExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_ConditionalExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_ConditionalExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_ConditionalExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<int, int, int>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_using_ConditionalExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        ParameterExpression x = Expression.Parameter(typeof(int), "x");
        ParameterExpression y = Expression.Parameter(typeof(int), "y");

        var expression = Expression.Lambda<Func<int, int, int>>(
            Expression.Condition(
                Expression.GreaterThan(x, y),
                x,
                y),
            x,
            y);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_first_when_greater()
    {
        var result1 = _originalExpression.Compile()(5, 3);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(5, 3);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(5, 3);

        5
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_second_when_less()
    {
        var result1 = _originalExpression.Compile()(3, 5);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(3, 5);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(3, 5);

        5
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_be_equal_when_equal()
    {
        var result1 = _originalExpression.Compile()(5, 5);
        var result2 = _remoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(5, 5);
        var result3 = _serializedRemoteExpression.ToLinqExpression<Func<int, int, int>>().Compile()(5, 5);

        5
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }
}
