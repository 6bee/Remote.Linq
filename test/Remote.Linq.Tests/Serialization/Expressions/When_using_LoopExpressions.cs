// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_LoopExpressions
    {
        public class BinaryFormatter : When_using_LoopExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class DataContractSerializer : When_using_LoopExpressions
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_LoopExpressions
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_LoopExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class ProtobufNetSerializer : When_using_LoopExpressions
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_LoopExpressions
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private readonly Expression<Func<int, int>> _originalExpression;

        private readonly RemoteLambdaExpression _remoteExpression;

        private readonly RemoteLambdaExpression _serializedRemoteExpression;

        [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
        protected When_using_LoopExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression maxRange = Expression.Parameter(typeof(int));

            ParameterExpression sum = Expression.Variable(typeof(int));
            ParameterExpression i = Expression.Variable(typeof(int));

            LabelTarget breakLabel = Expression.Label();

            var expression = Expression.Lambda<Func<int, int>>(
                Expression.Block(
                    new[] { sum, i },
                    Expression.Assign(sum, Expression.Constant(0)),
                    Expression.Assign(i, Expression.Constant(-1)),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.LessThan(i, maxRange),
                            Expression.Block(
                                Expression.PreIncrementAssign(i),
                                Expression.IfThen(
                                    Expression.Equal(Expression.Modulo(i, Expression.Constant(2)), Expression.Constant(0)),
                                    Expression.AddAssign(sum, i))),
                            Expression.Break(breakLabel)),
                        breakLabel),
                    sum), maxRange);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_result_should_be_equal()
        {
            var argument = 10;

            int int1 = _originalExpression.Compile()(argument);

            int int2 = _remoteExpression.ToLinqExpression<int, int>().Compile()(argument);

            int int3 = _serializedRemoteExpression.ToLinqExpression<int, int>().Compile()(argument);

            30
                .ShouldMatch(int1)
                .ShouldMatch(int2)
                .ShouldMatch(int3);
        }
    }
}