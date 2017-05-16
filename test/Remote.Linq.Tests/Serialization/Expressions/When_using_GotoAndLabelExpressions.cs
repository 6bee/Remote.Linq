// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.IO;

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;    
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_GotoAndLabelExpressions
    {
#if NET
        public class BinaryFormatter : When_using_GotoAndLabelExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class NetDataContractSerializer : When_using_GotoAndLabelExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif

        public class DataContractSerializer : When_using_GotoAndLabelExpressions
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_GotoAndLabelExpressions
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_GotoAndLabelExpressions
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.SerializeExpression)
            {
            }
        }

        private Expression<Func<StreamWriter, long>> _originalExpression;

        private RemoteLambdaExpression _remoteExpression;

        private RemoteLambdaExpression _serializedRemoteExpression;

        protected When_using_GotoAndLabelExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression writer = Expression.Parameter(typeof(StreamWriter));
            ParameterExpression position = Expression.Variable(typeof(long));

            MemberExpression positionProperty = Expression.PropertyOrField(Expression.PropertyOrField(writer, "BaseStream"), "Position");

            LabelTarget returnLabel = Expression.Label(typeof(long));

            var expression = Expression.Lambda<Func<StreamWriter, long>>
            (
                Expression.Block
                (
                    new[] { position },
                    Expression.Assign(position, positionProperty),
                    Expression.Call(writer, typeof(TextWriter).GetMethod(nameof(TextWriter.WriteLine), new[] { typeof(string) }), Expression.Constant("SomeText")),
                    Expression.Return(returnLabel, Expression.Subtract(positionProperty, position), typeof(long)),
                    Expression.Label(returnLabel, Expression.Default(typeof(long)))
                ), writer);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_block_result_should_be_equal()
        {
            var argument = StreamWriter.Null;
            
            long long1 = _originalExpression.Compile()(argument);

            long long2 = _remoteExpression.ToLinqExpression<StreamWriter, long>().Compile()(argument);

            long long3 = _serializedRemoteExpression.ToLinqExpression<StreamWriter, long>().Compile()(argument);

            0L
                .ShouldMatch(long1)
                .ShouldMatch(long2)
                .ShouldMatch(long3);
        }
    }
}