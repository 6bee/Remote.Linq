// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    /// <summary>
    /// Covers mapping type mismatches for unassignable types without validation, i.e. exeption upon assignment
    /// </summary>
    public class When_mapping_dynamic_object_to_type_with_different_property_types
    {
        class CustomType
        {
            public int Int32Value { get; set; }
        }

        DynamicObject dynamicObject;

        public When_mapping_dynamic_object_to_type_with_different_property_types()
        {
            dynamicObject = new DynamicObject
            {
                { "Int32Value", 1.23456789 },
            };
        }

        [Fact]
        public void Should_throw_when_preventing_type_validation()
        {
            var mapper = new DynamicObjectMapper(silentlySkipUnassignableMembers: false);

            var ex = Assert.Throws<Exception>(() => mapper.Map<CustomType>(dynamicObject));

            ex.InnerException.ShouldBeInstanceOf<ArgumentException>();
            ex.InnerException.Message.ShouldBe("Object of type 'System.Double' cannot be converted to type 'System.Int32'.");
        }

        [Fact]
        public void Should_silently_skip_unmatching_value_when_allowing_type_validation()
        {
            var mapper = new DynamicObjectMapper(silentlySkipUnassignableMembers: true);

            var obj = mapper.Map<CustomType>(dynamicObject);

            obj.ShouldNotBeNull();
        }
    }
}