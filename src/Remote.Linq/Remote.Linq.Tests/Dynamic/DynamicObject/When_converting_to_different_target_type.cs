// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_different_target_type
    {
        class SourceType
        {
            //public int Int32Value { get; set; }
            //public string StringValue { get; set; }
        }

        class TargetType
        {
            public int Int32Value { get; set; }
            public string StringValue { get; set; }
        }

        const int Int32Value = 11;
        const string StringValue = "eleven";

        TargetType obj;

        public When_converting_to_different_target_type()
        {
            var dynamicObject = new DynamicObject(typeof(SourceType))
            {
                { "StringValue", StringValue },
                { "Int32Value", Int32Value },
            };

            obj = dynamicObject.CreateObject(typeof(TargetType)) as TargetType;
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
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}