// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteExpression = Remote.Linq.Expressions.Expression;

    public abstract class When_using_IfElseExpressions
    {
        public class BinaryFormatter : When_using_IfElseExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_IfElseExpressions
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_IfElseExpressions
        {
            public JsonSerializer()
                : base(x => (RemoteExpression)JsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_IfElseExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

#if NETCOREAPP
        public class ProtobufNetSerializer : When_using_IfElseExpressions
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }
#endif // NETCOREAPP

        public class XmlSerializer : When_using_IfElseExpressions
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private readonly Expression _originalExpression;

        private readonly RemoteExpression _remoteExpression;

        private readonly RemoteExpression _serializedRemoteExpression;

        protected When_using_IfElseExpressions(Func<RemoteExpression, RemoteExpression> serialize)
        {
            var expression = Expression.IfThenElse(
                Expression.MakeBinary(ExpressionType.LessThan, Expression.Constant(5), Expression.Constant(2)),
                Expression.Throw(Expression.New(typeof(Exception).GetConstructor(new[] { typeof(string) }), Expression.Constant("The condition is true."))),
                Expression.Throw(Expression.New(typeof(Exception).GetConstructor(new[] { typeof(string) }), Expression.Constant("The condition is false."))));

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_result_should_be_equal()
        {
            static void Execute(Expression expression) => Expression.Lambda<Action>(expression).Compile()();

            Assert.Throws<Exception>(() => Execute(_originalExpression)).Message.ShouldMatch("The condition is false.");

            Assert.Throws<Exception>(() => Execute(_remoteExpression.ToLinqExpression())).Message.ShouldMatch("The condition is false.");

            Assert.Throws<Exception>(() => Execute(_serializedRemoteExpression.ToLinqExpression())).Message.ShouldMatch("The condition is false.");
        }
    }
}