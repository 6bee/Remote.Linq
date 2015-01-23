// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_anonymous_type
    {
        const int Int32Value = 11;
        const double DoubleValue = 1.234567891;
        const string StringValue = "eleven";

        DynamicObject dynamicObject;

        public When_converting_to_anonymous_type()
        {
            dynamicObject = new DynamicObject()
            {
                { "Int32Value", Int32Value },
                { "DoubleValue", DoubleValue },
                { "StringValue", StringValue },
            };
        }

        [Fact]
        public void Should_be_convertible_to_anonymous_type_with_same_property_order()
        {
            var objType = new { Int32Value, DoubleValue, StringValue };

            var obj = dynamicObject.CreateObject(objType.GetType());

            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_be_convertible_to_anonymous_type_with_less_properties()
        {
            var objType = new { Int32Value, StringValue };

            var obj = dynamicObject.CreateObject(objType.GetType());

            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_not_be_convertible_to_anonymous_type_with_aditional_properties()
        {
            var objType = new { Int32Value, DoubleValue, StringValue, DateTimeValue = DateTime.Now };

            var ex = Assert.Throws<Exception>(() => dynamicObject.CreateObject(objType.GetType()));

            Assert.True(ex.Message.StartsWith("Failed to pick matching contructor for type"));
        }

        [Fact]
        public void Should_be_convertible_to_anonymous_type_with_different_property_order()
        {
            var objType = new { StringValue, Int32Value, DoubleValue };

            var obj = dynamicObject.CreateObject(objType.GetType());

            obj.ShouldNotBeNull();
        }
    }
}