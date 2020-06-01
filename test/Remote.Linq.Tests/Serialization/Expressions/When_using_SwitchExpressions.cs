// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_SwitchExpressions
    {
        public class BinaryFormatter : When_using_SwitchExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_SwitchExpressions
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_SwitchExpressions
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFX
        public class NetDataContractSerializer : When_using_SwitchExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFX

#if COREFX
        public class ProtobufNetSerializer : When_using_SwitchExpressions
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }
#endif // COREFX

        public class XmlSerializer : When_using_SwitchExpressions
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private readonly Expression<Func<int, int>> _originalExpression;

        private readonly RemoteLambdaExpression _remoteExpression;

        private readonly RemoteLambdaExpression _serializedRemoteExpression;

        protected When_using_SwitchExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression switchOver = Expression.Parameter(typeof(int));
            ParameterExpression result = Expression.Variable(typeof(int));

            var expression = Expression.Lambda<Func<int, int>>(
                Expression.Block(
                    new[] { result },
                    Expression.Assign(result, Expression.Constant(0)),
                    Expression.Switch(
                        switchOver,
                        Expression.Assign(result, Expression.Constant(-1)),
                        Expression.SwitchCase(
                            Expression.Assign(result, Expression.Constant(1)),
                            Expression.Constant(0),
                            Expression.Constant(2)),
                        Expression.SwitchCase(
                            Expression.Assign(result, Expression.Constant(2)),
                            Expression.Constant(1),
                            Expression.Constant(3))),
                    result),
                switchOver);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_result_should_be_default()
        {
            int minusOne = -1;
            int maxValue = int.MaxValue;

            int intNegativeDefault1 = _originalExpression.Compile()(minusOne);
            int intMaxDefault1 = _originalExpression.Compile()(maxValue);

            int intNegativeDefault2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(minusOne);
            int intMaxDefault2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(maxValue);

            int intNegativeDefault3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(minusOne);
            int intMaxDefault3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(maxValue);

            (-1)
                .ShouldMatch(intNegativeDefault1)
                .ShouldMatch(intMaxDefault1)
                .ShouldMatch(intNegativeDefault2)
                .ShouldMatch(intMaxDefault2)
                .ShouldMatch(intNegativeDefault3)
                .ShouldMatch(intMaxDefault3);
        }

        [Fact]
        public void Expression_result_should_be_even()
        {
            int zero = 0;
            int two = 2;

            int intZeroEven1 = _originalExpression.Compile()(zero);
            int intTwoEven1 = _originalExpression.Compile()(two);

            int intZeroEven2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(zero);
            int intTwoEven2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(two);

            int intZeroEven3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(zero);
            int intTwoEven3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(two);

            1
                .ShouldMatch(intZeroEven1)
                .ShouldMatch(intTwoEven1)
                .ShouldMatch(intZeroEven2)
                .ShouldMatch(intTwoEven2)
                .ShouldMatch(intZeroEven3)
                .ShouldMatch(intTwoEven3);
        }

        [Fact]
        public void Expression_result_should_be_odd()
        {
            int one = 1;
            int three = 3;

            int intOneOdd1 = _originalExpression.Compile()(one);
            int intThreeOdd1 = _originalExpression.Compile()(three);

            int intOneOdd2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(one);
            int intThreeOdd2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(three);

            int intOneOdd3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(one);
            int intThreeOdd3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(three);

            2
                .ShouldMatch(intOneOdd1)
                .ShouldMatch(intThreeOdd1)
                .ShouldMatch(intOneOdd2)
                .ShouldMatch(intThreeOdd2)
                .ShouldMatch(intOneOdd3)
                .ShouldMatch(intThreeOdd3);
        }
    }
}