// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.ExpressionVisitors;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Xunit;
    using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

    public abstract class When_using_LambdaExpression_returnung_a_type
    {
        public class BinaryFormatter : When_using_LambdaExpression_returnung_a_type
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

#if NET
        public class NetDataContractSerializer : When_using_LambdaExpression_returnung_a_type
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif

        public class DataContractSerializer : When_using_LambdaExpression_returnung_a_type
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.SerializeExpression)
            {
            }
        }

        public class JsonSerializer : When_using_LambdaExpression_returnung_a_type
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }

        public class XmlSerializer : When_using_LambdaExpression_returnung_a_type
        {
            public XmlSerializer()
                : base(x => XmlSerializationHelper.SerializeExpression(x, new[] { typeof(List<Aqua.TypeSystem.TypeInfo>) }))
            {
            }
        }

        private readonly Func<RemoteLambdaExpression, RemoteLambdaExpression> _serialize;

        protected When_using_LambdaExpression_returnung_a_type(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
        {
            _serialize = exp => serialize(exp.ReplaceGenericQueryArgumentsByNonGenericArguments());
        }

        [Fact]
        public void Should_support_lambda_returning_typeof_int()
        {
            var type = typeof(int?[]);
            Expression<Func<Type>> transform = () => type;
            var expression = transform.ToRemoteLinqExpression();
            var serialized = _serialize(expression);
            var resurectedExpression = serialized.ToLinqExpression<Type>();
            resurectedExpression.Compile()().ShouldBe(type);
        }

        // [Theory]
        // [MemberData(nameof(TestData.Types), MemberType = typeof(TestData))]
        // public void Should_support_lambda_returning_type(Type type)
        // {
        //     Expression<Func<Type>> transform = () => type;
        //     var expression = transform.ToRemoteLinqExpression();
        //     var serialized = _serialize(expression);
        //     var resurectedExpression = serialized.ToLinqExpression<Type>();
        //     resurectedExpression.Compile()().ShouldBe(type);
        // }
    }
}