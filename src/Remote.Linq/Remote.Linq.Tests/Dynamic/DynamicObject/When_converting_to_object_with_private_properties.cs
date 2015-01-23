// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Reflection;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_object_with_private_properties
    {
        class CustomType
        {
            public int Int32Value { get; set; }
            private double DoubleValue { get; set; }
            private string StringValue { get; set; }
        }

        const int Int32Value = 11;
        const double DoubleValue = 12.3456789;
        const string StringValue = "eleven";

        CustomType obj;

        public When_converting_to_object_with_private_properties()
        {
            var dynamicObject = new DynamicObject
            {
                { "Int32Value", Int32Value },
                { "DoubleValue", DoubleValue },
                { "StringValue", StringValue },
            };

            obj = dynamicObject.CreateObject<CustomType>();
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
            GetPropertyValue("DoubleValue").ShouldBe(default(double));
        }

        [Fact]
        public void Should_have_the_string_property_not_set()
        {
            GetPropertyValue("StringValue").ShouldBeNull();
        }

        private object GetPropertyValue(string propertyName)
        {
            return typeof(CustomType).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
    }
}