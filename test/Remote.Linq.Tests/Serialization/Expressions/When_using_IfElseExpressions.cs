// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteExpression = Remote.Linq.Expressions.Expression;

    public abstract class When_using_IfElseExpressions
    {
        public class With_binary_formatter : When_using_IfElseExpressions
        {
            public With_binary_formatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class With_data_contract_serializer : When_using_IfElseExpressions
        {
            public With_data_contract_serializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class With_newtonsoft_json_serializer : When_using_IfElseExpressions
        {
            public With_newtonsoft_json_serializer()
                : base(x => (RemoteExpression)NewtonsoftJsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }

        public class With_system_text_json_serializer : When_using_IfElseExpressions
        {
            public With_system_text_json_serializer()
                : base(x => (RemoteExpression)SystemTextJsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }

#if NETFRAMEWORK
        public class With_net_data_contract_serializer : When_using_IfElseExpressions
        {
            public With_net_data_contract_serializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class With_protobuf_net_serializer : When_using_IfElseExpressions
        {
            public With_protobuf_net_serializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class With_xml_serializer : When_using_IfElseExpressions
        {
            public With_xml_serializer()
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