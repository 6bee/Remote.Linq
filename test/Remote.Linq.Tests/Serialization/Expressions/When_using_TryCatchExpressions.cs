// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_TryCatchExpressions
    {
#pragma warning disable SA1502 // Element should not be on a single line
#pragma warning disable SA1128 // Put constructor initializers on their own line

#if NET
        public class BinaryFormatter : When_using_TryCatchExpressions
        {
            public BinaryFormatter() : base(BinarySerializationHelper.Serialize) { }
        }
#endif

#if NET && !NETCOREAPP2
        public class NetDataContractSerializer : When_using_TryCatchExpressions
        {
            public NetDataContractSerializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

        public class DataContractSerializer : When_using_TryCatchExpressions
        {
            public DataContractSerializer() : base(DataContractSerializationHelper.SerializeExpression) { }
        }

        public class JsonSerializer : When_using_TryCatchExpressions
        {
            public JsonSerializer() : base(JsonSerializationHelper.Serialize) { }
        }

        public class XmlSerializer : When_using_TryCatchExpressions
        {
            public XmlSerializer() : base(XmlSerializationHelper.SerializeExpression) { }
        }

        #pragma warning restore SA1128 // Put constructor initializers on their own line
#pragma warning restore SA1502 // Element should not be on a single line

        private Expression<Func<bool, bool>> _originalExpression;

        private RemoteLambdaExpression _remoteExpression;

        private RemoteLambdaExpression _serializedRemoteExpression;

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