// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using Remote.Linq.ExpressionVisitors;
using Shouldly;
using System;
using System.Linq.Expressions;
using Xunit;
using RemoteExpression = Remote.Linq.Expressions.Expression;

// NOTES:
// protobuf-net serializer: model not supporting const expression of expression
// xml serializer: not supporting expression array
public abstract class When_using_ConstExpression_of_expression
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter() : When_using_ConstExpression_of_expression(BinarySerializationHelper.Clone);
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer() : When_using_ConstExpression_of_expression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_ConstExpression_of_expression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_ConstExpression_of_expression(SystemTextJsonSerializationHelper.Clone);

#if NETFRAMEWORK
    public class With_net_data_contract_serializer() : When_using_ConstExpression_of_expression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Func<RemoteExpression, RemoteExpression> _serialize;

    protected When_using_ConstExpression_of_expression(Func<RemoteExpression, RemoteExpression> serialize)
        => _serialize = serialize;

    [Fact]
    public void ConstExpression_of_ExpressionArray_should_be_serializable()
    {
        Expression[] expressions = [Expression.Constant(1)];
        Expression constExpression = Expression.Constant(expressions);

        var clone = Serialize(constExpression);

        var c1 = clone.ShouldBeOfType<ConstantExpression>();
        var c2 = c1.Value.ShouldBeOfType<Expression[]>();
        var c3 = c2.ShouldHaveSingleItem();
        var c4 = c3.ShouldBeOfType<ConstantExpression>();
        c4.Value.ShouldBe(1);
    }

    [Fact]
    public void ConstExpression_of_ConstExpression_should_be_serializable()
    {
        Expression expression = Expression.Constant(1);
        Expression constExpression = Expression.Constant(expression);

        var clone = Serialize(constExpression);

        var c1 = clone.ShouldBeAssignableTo<ConstantExpression>();
        var c2 = c1.Value.ShouldBeAssignableTo<ConstantExpression>();
        c2.Value.ShouldBe(1);
    }

    private Expression Serialize(Expression constExpression)
    {
        var remoteExpression = constExpression
            .ToRemoteLinqExpression()
            .ReplaceGenericQueryArgumentsByNonGenericArguments();

        var serializedRemoteExpression = _serialize(remoteExpression);

        serializedRemoteExpression.ShouldNotBeNull();

        return serializedRemoteExpression.ToLinqExpression();
    }
}