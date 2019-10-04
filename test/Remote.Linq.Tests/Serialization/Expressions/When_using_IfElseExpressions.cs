// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteExpression = Remote.Linq.Expressions.Expression;

    public abstract class When_using_IfElseExpressions
    {
#pragma warning disable SA1502 // Element should not be on a single line
#pragma warning disable SA1128 // Put constructor initializers on their own line

        public class BinaryFormatter : When_using_IfElseExpressions
        {
            public BinaryFormatter() : base(BinarySerializationHelper.Serialize) { }
        }

#if NET
        public class NetDataContractSerializer : When_using_IfElseExpressions
        {
            public NetDataContractSerializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

        public class DataContractSerializer : When_using_IfElseExpressions
        {
            public DataContractSerializer() : base(DataContractSerializationHelper.SerializeExpression) { }
        }

        public class JsonSerializer : When_using_IfElseExpressions
        {
            public JsonSerializer() : base(x => (RemoteExpression)JsonSerializationHelper.Serialize(x, x.GetType())) { }
        }

        public class XmlSerializer : When_using_IfElseExpressions
        {
            public XmlSerializer() : base(XmlSerializationHelper.SerializeExpression) { }
        }

#pragma warning restore SA1128 // Put constructor initializers on their own line
#pragma warning restore SA1502 // Element should not be on a single line

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