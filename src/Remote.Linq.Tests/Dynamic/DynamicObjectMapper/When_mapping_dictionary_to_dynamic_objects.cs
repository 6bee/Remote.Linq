// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_dictionary_to_dynamic_objects
    {
        Dictionary<string, string> source;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_dictionary_to_dynamic_objects()
        {
            source = new Dictionary<string, string>
            {
                { "K1", "V1" },
                { "K2", "V2" },
                { "K3", "V3" },
            };
            dynamicObjects = DynamicObjectMapper.Map(source);
        }

        [Fact]
        public void Dynamic_objects_count_should_be_three()
        {
            dynamicObjects.Count().ShouldBe(3);
        }

        [Fact]
        public void Dynamic_objects_type_property_should_be_set_to_keyvaluepair()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.Type.Type.ShouldBe(typeof(KeyValuePair<string, string>));
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
        public void Dynamic_objects_member_names_should_be_key_and_value()
        {
            foreach (var dynamicObject in dynamicObjects)
            {
                dynamicObject.MemberNames.ShouldContain("Key");
                dynamicObject.MemberNames.ShouldContain("Value");
            }
        }

        [Fact]
        public void Dynamic_objects_member_values_should_be_key_and_value_of_source()
        {
            var dynamicObject = dynamicObjects.First();
            dynamicObject["Key"].ShouldBe("K1");
            dynamicObject["Value"].ShouldBe("V1");
        }
    }
}
