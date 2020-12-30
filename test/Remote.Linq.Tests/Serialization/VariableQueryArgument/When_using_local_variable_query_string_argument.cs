// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public abstract class When_using_local_variable_query_string_argument
    {
        public class BinaryFormatter : When_using_local_variable_query_string_argument
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_local_variable_query_string_argument
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_local_variable_query_string_argument
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_local_variable_query_string_argument
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class ProtobufNetSerializer : When_using_local_variable_query_string_argument
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_local_variable_query_string_argument
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private class AType
        {
            public string Key { get; set; }
        }

        private readonly LambdaExpression _remoteExpression;

        private readonly LambdaExpression _serializedRemoteExpression;

        protected When_using_local_variable_query_string_argument(Func<LambdaExpression, LambdaExpression> serialize)
        {
            var key = "K1";

            System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => x.Key == key;

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
            var exp1 = _remoteExpression.ToLinqExpression<AType, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, bool>();

            exp1.ShouldEqualExpression(exp2);
        }
    }
}