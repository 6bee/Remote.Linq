// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_from_list_of_custom_reference_type
    {
        class CustomReferenceType
        {
            public int Int32Property { get; set; }
            public string StringProperty { get; set; }
        }

        List<CustomReferenceType> source;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_from_list_of_custom_reference_type()
        {
            source = new List<CustomReferenceType> 
            { 
                new CustomReferenceType { Int32Property = 1, StringProperty="One" },
                new CustomReferenceType { Int32Property = 2, StringProperty="Two" },
            };
            dynamicObjects = new DynamicObjectMapper().MapCollection(source);
        }

        [Fact]
        public void Dynamic_objects_count_should_be_two()
        {
            dynamicObjects.Count().ShouldBe(2);
        }

        [Fact]
        public void Dynamic_objects_type_property_should_be_set_to_custom_reference_type()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.Type.Type.ShouldBe(typeof(CustomReferenceType));
            }
        }

        [Fact]
        public void Dynamic_objects_should_have_two_members()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberCount.ShouldBe(2);
            }
        }

        [Fact]
        public void Dynamic_objects_member_names_should_be_property_names()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberNames.ElementAt(0).ShouldBe("Int32Property");
                dynamicObject.MemberNames.ElementAt(1).ShouldBe("StringProperty");
            }
        }

        [Fact]
        public void Dynamic_objects_member_values_should_be_key_and_value_of_source()
        {
            for (int i = 0; i < source.Count; i++)
            {
                var dynamicObject = dynamicObjects.ElementAt(i);

                var intValue = source.ElementAt(i).Int32Property;
                var stringValue = source.ElementAt(i).StringProperty;

                dynamicObject["Int32Property"].ShouldBe(intValue);
                dynamicObject["StringProperty"].ShouldBe(stringValue);
            }
        }
    }
}
