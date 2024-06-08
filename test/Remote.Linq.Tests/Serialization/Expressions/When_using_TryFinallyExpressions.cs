// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_TryFinallyExpressions
{
#if !NET8_0_OR_GREATER
    public class With_binary_formatter : When_using_TryFinallyExpressions
    {
        public With_binary_formatter()
            : base(BinarySerializationHelper.Clone)
        {
        }
    }
#endif // NET8_0_OR_GREATER

    public class With_data_contract_serializer : When_using_TryFinallyExpressions
    {
        public With_data_contract_serializer()
            : base(DataContractSerializationHelper.CloneExpression)
        {
        }
    }

    public class With_newtonsoft_json_serializer : When_using_TryFinallyExpressions
    {
        public With_newtonsoft_json_serializer()
            : base(NewtonsoftJsonSerializationHelper.Clone)
        {
        }
    }

    public class With_system_text_json_serializer : When_using_TryFinallyExpressions
    {
        public With_system_text_json_serializer()
            : base(SystemTextJsonSerializationHelper.Clone)
        {
        }
    }

#if NETFRAMEWORK
    public class With_net_data_contract_serializer : When_using_TryFinallyExpressions
    {
        public With_net_data_contract_serializer()
            : base(NetDataContractSerializationHelper.Clone)
        {
        }
    }
#endif // NETFRAMEWORK

    public class With_protobuf_net_serializer : When_using_TryFinallyExpressions
    {
        public With_protobuf_net_serializer()
            : base(ProtobufNetSerializationHelper.Clone)
        {
        }
    }

    public class With_xml_serializer : When_using_TryFinallyExpressions
    {
        public With_xml_serializer()
            : base(XmlSerializationHelper.CloneExpression)
        {
        }
    }

    private readonly Expression<Func<bool, bool>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Test setup")]
    protected When_using_TryFinallyExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        ParameterExpression shouldFail = Expression.Parameter(typeof(bool));
        ParameterExpression result = Expression.Variable(typeof(bool));

        var expression = Expression.Lambda<Func<bool, bool>>(
            Expression.Block(
                new[] { result },
                Expression.TryFinally(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(Expression.Constant(true), shouldFail),
                            Expression.Throw(Expression.New(typeof(InvalidOperationException).GetTypeInfo().GetConstructor([typeof(string)]), Expression.Constant("x"))))),
                    Expression.Throw(Expression.New(typeof(InvalidOperationException).GetTypeInfo().GetConstructor([typeof(string)]), Expression.Constant("y")))),
                Expression.Assign(result, Expression.Constant(true)),
                result),
            shouldFail);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_be_equal()
    {
        Assert.Throws<InvalidOperationException>(() => _originalExpression.Compile()(true)).Message.ShouldMatch("y");
        Assert.Throws<InvalidOperationException>(() => _originalExpression.Compile()(false)).Message.ShouldMatch("y");

        Assert.Throws<InvalidOperationException>(() => _remoteExpression.ToLinqExpression<bool, bool>().Compile()(true)).Message.ShouldMatch("y");
        Assert.Throws<InvalidOperationException>(() => _remoteExpression.ToLinqExpression<bool, bool>().Compile()(false)).Message.ShouldMatch("y");

        Assert.Throws<InvalidOperationException>(() => _serializedRemoteExpression.ToLinqExpression<bool, bool>().Compile()(true)).Message.ShouldMatch("y");
        Assert.Throws<InvalidOperationException>(() => _serializedRemoteExpression.ToLinqExpression<bool, bool>().Compile()(false)).Message.ShouldMatch("y");
    }
}