// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_from_list_of_known_types
    {
        class CustomReferenceType
        {
            public int Int32Property { get; set; }
            public string StringProperty { get; set; }
        }

        CustomReferenceType[] sourceObjects;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_from_list_of_known_types()
        {
            sourceObjects = new[]
            { 
                new CustomReferenceType { Int32Property = 1, StringProperty="One" },
                new CustomReferenceType { Int32Property = 2, StringProperty="Two" },
            };

            var mapper = new DynamicObjectMapper(knownTypes: new[] { typeof(CustomReferenceType) });
            
            dynamicObjects = mapper.MapCollection(sourceObjects);
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
        public void Dynamic_objects_should_have_one_member_only()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberCount.ShouldBe(1);
            }
        }

        [Fact]
        public void Dynamic_objects_member_name_should_be_empty_strings()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberNames.Single().ShouldBeEmpty();
            }
        }

        [Fact]
        public void Dynamic_objects_member_value_should_be_source_objects()
        {
            for (int i = 0; i < sourceObjects.Length; i++)
            {
                var dynamicObject = dynamicObjects.ElementAt(i);

                var sourceObject = sourceObjects.ElementAt(i);

                dynamicObject[""].ShouldBeSameAs(sourceObject);
            }
        }
    }
}
