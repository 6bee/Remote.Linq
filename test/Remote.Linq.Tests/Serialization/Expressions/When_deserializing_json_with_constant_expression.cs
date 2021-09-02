// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using global::Newtonsoft.Json;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Shouldly;
    using Xunit;

    public class When_deserializing_json_with_constant_expression
    {
        private static readonly string Json = $@"
{{
    ""$id"": ""1"",
    ""$type"": ""Remote.Linq.Expressions.ConstantExpression, Remote.Linq"",
    ""Type"": {{
        ""$id"": ""2"",
        ""Name"": ""QueryableResourceDescriptor"",
        ""Namespace"": ""Remote.Linq.DynamicQuery""
    }},
    ""Value"": {{
        ""$type"": ""Remote.Linq.DynamicQuery.QueryableResourceDescriptor, Remote.Linq"",
        ""Type"": {{
            ""Name"": ""{nameof(When_deserializing_json_with_constant_expression)}"",
            ""Namespace"": ""{typeof(When_deserializing_json_with_constant_expression).Namespace}""
        }}
    }}
}}";

        private readonly ConstantExpression expression;

        public When_deserializing_json_with_constant_expression()
        {
            var serializerSettings = NewtonsoftJsonSerializationHelper.SerializerSettings;
            expression = JsonConvert.DeserializeObject<ConstantExpression>(Json, serializerSettings);
        }

        [Fact]
        public void Expresison_type_should_be_QueryableResourceDescriptor()
        {
            expression.Type.Name.ShouldBe(nameof(QueryableResourceDescriptor));
            expression.Type.Namespace.ShouldBe(typeof(QueryableResourceDescriptor).Namespace);
        }

        [Fact]
        public void Expresison_value_should_be_instance_of_QueryableResourceDescriptor()
        {
            expression.Value.ShouldBeOfType<QueryableResourceDescriptor>();
        }

        [Fact]
        public void Type_property_of_QueryableResourceDescriptor_should_be_set()
        {
            var queryableResourceDescriptor = expression.Value.ShouldBeOfType<QueryableResourceDescriptor>();
            queryableResourceDescriptor.Type.Name.ShouldBe(nameof(When_deserializing_json_with_constant_expression));
            queryableResourceDescriptor.Type.Namespace.ShouldBe(typeof(When_deserializing_json_with_constant_expression).Namespace);
        }
    }
}