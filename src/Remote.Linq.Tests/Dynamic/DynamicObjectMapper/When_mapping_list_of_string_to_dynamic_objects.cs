// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_list_of_string_to_dynamic_objects
    {
        List<string> source;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_list_of_string_to_dynamic_objects()
        {
            source = new List<string> { "V1", "V2", "V3" };
            dynamicObjects = DynamicObjectMapper.Map(source);
        }

        [Fact]
        public void Dynamic_objects_count_should_be_three()
        {
            dynamicObjects.Count().ShouldBe(3);
        }

        [Fact]
        public void Dynamic_objects_type_property_should_be_set_to_string()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.Type.Type.ShouldBe(typeof(string));
            }
        }

        [Fact]
        public void Dynamic_objects_should_have_two_members()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberCount.ShouldBe(1);
            }
        }

        [Fact]
        public void Dynamic_objects_member_names_should_be_key_and_value()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberNames.Single().ShouldBe(string.Empty);
            }
        }

        [Fact]
        public void Dynamic_objects_member_values_should_be_key_and_value_of_source()
        {
            dynamicObjects.Skip(0).Take(1).Single()[string.Empty].ShouldBe("V1");
            dynamicObjects.Skip(1).Take(1).Single()[string.Empty].ShouldBe("V2");
            dynamicObjects.Skip(2).Take(1).Single()[string.Empty].ShouldBe("V3");
        }
    }
}
