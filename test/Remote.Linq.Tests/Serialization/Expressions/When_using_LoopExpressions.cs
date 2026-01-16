// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_LoopExpressions
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter() : When_using_LoopExpressions(BinarySerializationHelper.Clone);
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer() : When_using_LoopExpressions(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_LoopExpressions(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_LoopExpressions(SystemTextJsonSerializationHelper.Clone);

#if NETFRAMEWORK
    public class With_net_data_contract_serializer() : When_using_LoopExpressions(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer() : When_using_LoopExpressions(ProtobufNetSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_LoopExpressions(XmlSerializationHelper.CloneExpression);

    private readonly Expression<Func<int, int>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_using_LoopExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        ParameterExpression maxRange = Expression.Parameter(typeof(int));

        ParameterExpression sum = Expression.Variable(typeof(int));
        ParameterExpression i = Expression.Variable(typeof(int));

        LabelTarget breakLabel = Expression.Label();

        var expression = Expression.Lambda<Func<int, int>>(
            Expression.Block(
                new[] { sum, i },
                Expression.Assign(sum, Expression.Constant(0)),
                Expression.Assign(i, Expression.Constant(-1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(i, maxRange),
                        Expression.Block(
                            Expression.PreIncrementAssign(i),
                            Expression.IfThen(
                                Expression.Equal(Expression.Modulo(i, Expression.Constant(2)), Expression.Constant(0)),
                                Expression.AddAssign(sum, i))),
                        Expression.Break(breakLabel)),
                    breakLabel),
                sum),
            maxRange);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_equal()
    {
        var argument = 10;

        int int1 = _originalExpression.Compile()(argument);

        int int2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(argument);

        int int3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(argument);

        30
            .ShouldMatch(int1)
            .ShouldMatch(int2)
            .ShouldMatch(int3);
    }
}