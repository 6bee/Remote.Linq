// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public abstract class When_using_local_variable_query_argument_list
    {
        public class With_binary_formatter : When_using_local_variable_query_argument_list
        {
            public With_binary_formatter()
                : base(BinarySerializationHelper.Clone)
            {
            }
        }

        public class With_data_contract_serializer : When_using_local_variable_query_argument_list
        {
            public With_data_contract_serializer()
                : base(x => DataContractSerializationHelper.CloneExpression(x, new[] { typeof(List<int>) }))
            {
            }
        }

        public class With_newtonsoft_json_serializer : When_using_local_variable_query_argument_list
        {
            public With_newtonsoft_json_serializer()
                : base(NewtonsoftJsonSerializationHelper.Clone)
            {
            }
        }

        public class With_system_text_json_serializer : When_using_local_variable_query_argument_list
        {
            public With_system_text_json_serializer()
                : base(SystemTextJsonSerializationHelper.Clone)
            {
            }
        }

#if NETFRAMEWORK
        public class With_net_data_contract_serializer : When_using_local_variable_query_argument_list
        {
            public With_net_data_contract_serializer()
                : base(NetDataContractSerializationHelper.Clone)
            {
            }
        }
#endif // NETFRAMEWORK

        public class With_protobuf_net_serializer : When_using_local_variable_query_argument_list
        {
            public With_protobuf_net_serializer()
                : base(ProtobufNetSerializationHelper.Clone)
            {
            }
        }

        public class With_xml_serializer : When_using_local_variable_query_argument_list
        {
            public With_xml_serializer()
                : base(x => XmlSerializationHelper.CloneExpression(x, new[] { typeof(List<int>) }))
            {
            }
        }

        public class AType
        {
            public int Number { get; set; }
        }

        private readonly LambdaExpression _remoteExpression;

        private readonly LambdaExpression _serializedRemoteExpression;

        protected When_using_local_variable_query_argument_list(Func<LambdaExpression, LambdaExpression> serialize)
        {
            var values = new List<int> { 123, 456, 789 };

            System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => values.Contains(x.Number);

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
