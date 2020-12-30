// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public abstract class When_using_const_bool_expression
    {
        public class NoSerialization : When_subquery_expression_use_same_parameter_name
        {
            public NoSerialization()
                : base(x => x)
            {
            }
        }

        public class BinaryFormatter : When_using_const_bool_expression
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_const_bool_expression
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_const_bool_expression
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_const_bool_expression
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class ProtobufNetSerializer : When_using_const_bool_expression
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_const_bool_expression
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private readonly LambdaExpression _remoteExpression;

        private readonly LambdaExpression _serializedRemoteExpression;

        protected When_using_const_bool_expression(Func<LambdaExpression, LambdaExpression> serialize)
        {
            System.Linq.Expressions.Expression<Func<bool, bool>> expression = x => false;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Remote_expression_should_be_equal()
        {
            _remoteExpression.ShouldEqualRemoteExpression(_serializedRemoteExpression);
        }

        [Fact]
        public void System_expresison_should_be_equal()
        {
            var exp1 = _remoteExpression.ToLinqExpression<bool, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<bool, bool>();

            exp1.ShouldEqualExpression(exp2);
        }
    }
}