// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_LoopExpressions
    {
#pragma warning disable SA1502 // Element should not be on a single line
#pragma warning disable SA1128 // Put constructor initializers on their own line

        public class BinaryFormatter : When_using_LoopExpressions
        {
            public BinaryFormatter() : base(BinarySerializationHelper.Serialize) { }
        }

#if NET
        public class NetDataContractSerializer : When_using_LoopExpressions
        {
            public NetDataContractSerializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

        public class DataContractSerializer : When_using_LoopExpressions
        {
            public DataContractSerializer() : base(DataContractSerializationHelper.SerializeExpression) { }
        }

        public class JsonSerializer : When_using_LoopExpressions
        {
            public JsonSerializer() : base(JsonSerializationHelper.Serialize) { }
        }

        public class XmlSerializer : When_using_LoopExpressions
        {
            public XmlSerializer() : base(XmlSerializationHelper.SerializeExpression) { }
        }

        #pragma warning restore SA1128 // Put constructor initializers on their own line
#pragma warning restore SA1502 // Element should not be on a single line

        private Expression<Func<int, int>> _originalExpression;

        private RemoteLambdaExpression _remoteExpression;

        private RemoteLambdaExpression _serializedRemoteExpression;

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