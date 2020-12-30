// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_GotoAndLabelExpressions
    {
        public class BinaryFormatter : When_using_GotoAndLabelExpressions
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

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

#if NETFRAMEWORK
        public class NetDataContractSerializer : When_using_GotoAndLabelExpressions
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class ProtobufNetSerializer : When_using_GotoAndLabelExpressions
        {
            public ProtobufNetSerializer()
                : base(ProtobufNetSerializationHelper.Serialize)
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

        private readonly Expression<Func<StreamWriter, long>> _originalExpression;

        private readonly RemoteLambdaExpression _remoteExpression;

        private readonly RemoteLambdaExpression _serializedRemoteExpression;

        [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
        protected When_using_GotoAndLabelExpressions(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            ParameterExpression writer = Expression.Parameter(typeof(StreamWriter));
            ParameterExpression position = Expression.Variable(typeof(long));

            MemberExpression positionProperty = Expression.PropertyOrField(Expression.PropertyOrField(writer, "BaseStream"), "Position");

            LabelTarget returnLabel = Expression.Label(typeof(long));

            var expression = Expression.Lambda<Func<StreamWriter, long>>(
                Expression.Block(
                    new[] { position },
                    Expression.Assign(position, positionProperty),
                    Expression.Call(writer, typeof(TextWriter).GetMethod(nameof(TextWriter.WriteLine), new[] { typeof(string) }), Expression.Constant("SomeText")),
                    Expression.Return(returnLabel, Expression.Subtract(positionProperty, position), typeof(long)),
                    Expression.Label(returnLabel, Expression.Default(typeof(long)))), writer);

            _originalExpression = expression;

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Expression_result_should_be_equal()
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