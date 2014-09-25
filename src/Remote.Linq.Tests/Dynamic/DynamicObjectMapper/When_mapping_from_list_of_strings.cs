// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_from_list_of_strings
    {
        List<string> source;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_from_list_of_strings()
        {
            source = new List<string> { "V1", "V2", "V3" };
            dynamicObjects = DynamicObjectMapper.InstanceProvider().MapCollection(source);
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
        public void Dynamic_objects_should_have_one_member()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberCount.ShouldBe(1);
            }
        }

        [Fact]
        public void Dynamic_objects_member_name_should_be_empty_string()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberNames.Single().ShouldBe(string.Empty);
            }
        }

        [Fact]
        public void Dynamic_objects_member_values_should_be_value_of_source()
        {
            for (int i = 0; i < source.Count; i++)
            {
                var dynamicObject = dynamicObjects.ElementAt(i);
                var value = source.ElementAt(i);

                dynamicObject[string.Empty].ShouldBe(value);
            }
        }
    }
}
