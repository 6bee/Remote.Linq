// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_serializable_object_with_additional_properies
    {
        [Serializable]
        class SerializableType
        {
            public int Int32Value { get; set; }
            public double DoubleValue { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public string StringValue { get; set; }
        }

        const int Int32Value = 11;
        const string StringValue = "eleven";

        SerializableType obj;

        public When_converting_to_serializable_object_with_additional_properies()
        {
            var dynamicObject = new DynamicObject()
            {
                { "Int32Value", Int32Value },
                { "StringValue", StringValue },
            };

            obj = dynamicObject.CreateObject<SerializableType>();
        }

        [Fact]
        public void Should_create_an_instance()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_have_the_int_property_set()
        {
            obj.Int32Value.ShouldBe(Int32Value);
        }

        [Fact]
        public void Should_have_the_double_property_not_set()
        {
            obj.DoubleValue.ShouldBe(default(double));
        }

        [Fact]
        public void Should_have_the_date_property_not_set()
        {
            obj.NullableDateTime.ShouldBeNull();
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}
