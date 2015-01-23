// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    /// <summary>
    /// Covers mapping type missmatches for assignable types, i.e. no casting required
    /// </summary>
    public class When_converting_to_object_with_different_property_types_assignable
    {
        class CustomType
        {
            public double DoubleValue { get; set; }
            public int? NullableIntValue { get; set; }
            public object Timestamp { get; set; }
            public string StringValue { get; set; }
        }

        const int Int32Value = 11;
        readonly DateTime Timestamp = DateTime.Now;
        const string StringValue = "eleven";

        CustomType obj;

        public When_converting_to_object_with_different_property_types_assignable()
        {
            var dynamicObject = new DynamicObject
            {
                { "DoubleValue", Int32Value },
                { "NullableIntValue", Int32Value },
                { "Timestamp", Timestamp },
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
        public void Should_have_the_double_property_set()
        {
            obj.DoubleValue.ShouldBe(Int32Value);
        }

        [Fact]
        public void Should_have_the_nullableint_property_set()
        {
            obj.NullableIntValue.ShouldBe(Int32Value);
        }

        [Fact]
        public void Should_have_the_object_property_set()
        {
            obj.Timestamp.ShouldBe(Timestamp);
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}