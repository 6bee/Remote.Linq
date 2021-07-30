// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_TryCatchExpressions
    {
        public class With_binary_formatter : When_using_TryCatchExpressions
        {
            public With_binary_formatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class With_data_contract_serializer : When_using_TryCatchExpressions
        {
            public With_data_contract_serializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class With_newtonsoft_json_serializer : When_using_TryCatchExpressions
        {
            public With_newtonsoft_json_serializer()
                : base(NewtonsoftJsonSerializationHelper.Serialize)
            {
            }
        }

        public class With_system_text_json_serializer : When_using_TryCatchExpressions
        {
            public With_system_text_json_serializer()
                : base(SystemTextJsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFRAMEWORK
        public class With_net_data_contract_serializer : When_using_TryCatchExpressions
        {
            public With_net_data_contract_serializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class With_protobuf_net_serializer : When_using_TryCatchExpressions
        {
            public With_protobuf_net_serializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class With_xml_serializer : When_using_TryCatchExpressions
        {
            public With_xml_serializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private readonly Expression<Func<bool, bool>> _originalExpression;

        private readonly RemoteLambdaExpression _remoteExpression;

        private readonly RemoteLambdaExpression _serializedRemoteExpression;

        [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
        protected When_using_TryCatchExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression shouldFail = Expression.Parameter(typeof(bool));
            ParameterExpression result = Expression.Variable(typeof(bool));

            var expression = Expression.Lambda<Func<bool, bool>>(
                Expression.Block(
                    new[] { result },
                    Expression.TryCatch(
                        Expression.Block(
                            Expression.IfThen(
                                Expression.Equal(Expression.Constant(true), shouldFail),
                                Expression.Throw(Expression.New(typeof(InvalidOperationException)))),
                            Expression.Assign(result, Expression.Constant(true))),
                        Expression.Catch(
                            typeof(InvalidOperationException),
                            Expression.Assign(result, Expression.Constant(false)))),
                    result),
                shouldFail);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_result_should_be_equal()
        {
            bool fail = true;
            bool success = false;

            bool boolFail1 = _originalExpression.Compile()(fail);
            bool boolSuccess1 = _originalExpression.Compile()(success);

            bool boolFail2 = _remoteExpression.ToLinqExpression<bool, bool>().Compile()(fail);
            bool boolSuccess2 = _remoteExpression.ToLinqExpression<bool, bool>().Compile()(success);

            bool boolFail3 = _serializedRemoteExpression.ToLinqExpression<bool, bool>().Compile()(fail);
            bool boolSuccess3 = _serializedRemoteExpression.ToLinqExpression<bool, bool>().Compile()(success);

            false
                .ShouldMatch(boolFail1)
                .ShouldMatch(boolFail2)
                .ShouldMatch(boolFail3);

            true
                .ShouldMatch(boolSuccess1)
                .ShouldMatch(boolSuccess2)
                .ShouldMatch(boolSuccess3);
        }
    }
}