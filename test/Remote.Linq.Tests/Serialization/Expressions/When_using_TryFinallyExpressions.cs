// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_TryFinallyExpressions
    {
#if NET
        public class BinaryFormatter : When_using_TryFinallyExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }
#endif

#if NET && !NETCOREAPP2
        public class NetDataContractSerializer : When_using_TryFinallyExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif

        public class DataContractSerializer : When_using_TryFinallyExpressions
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_TryFinallyExpressions
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_TryFinallyExpressions
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private Expression<Func<bool, bool>> _originalExpression;

        private RemoteLambdaExpression _remoteExpression;

        private RemoteLambdaExpression _serializedRemoteExpression;

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
                                Expression.Throw(Expression.New(typeof(InvalidOperationException).GetTypeInfo().GetConstructor(new[] { typeof(string) }), Expression.Constant("x"))))),
                        Expression.Throw(Expression.New(typeof(InvalidOperationException).GetTypeInfo().GetConstructor(new[] { typeof(string) }), Expression.Constant("y")))),
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
}