// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Reflection;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_serializable_object_with_private_properties
    {
        [Serializable]
        class SerializableType
        {
            public int Int32Value { get; set; }
            private double DoubleValue { get; set; }
            private string StringValue { get; set; }
        }

        const int Int32Value = 11;
        const double DoubleValue = 12.3456789;
        const string StringValue = "eleven";

        SerializableType obj;

        public When_converting_to_serializable_object_with_private_properties()
        {
            var dynamicObject = new DynamicObject
            {
                { "Int32Value", Int32Value },
                { "DoubleValue", DoubleValue },
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
        public void Should_have_the_double_property_set()
        {
            GetPropertyValue("DoubleValue").ShouldBe(DoubleValue);
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            GetPropertyValue("StringValue").ShouldBe(StringValue);
        }

        private object GetPropertyValue(string propertyName)
        {
            return typeof(SerializableType).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}
