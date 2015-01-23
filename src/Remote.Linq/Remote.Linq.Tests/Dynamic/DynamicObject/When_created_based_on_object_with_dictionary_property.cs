// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_object_with_dictionary_property
    {
        class ClassWithDictionaryProperty
        {
            public IDictionary<string, string> Dictionary { get; set; }
        }

        ClassWithDictionaryProperty source;
        DynamicObject dynamicObject;

        public When_created_based_on_object_with_dictionary_property()
        {
            source = new ClassWithDictionaryProperty
            {
                Dictionary = new Dictionary<string, string>
                {
                    { "K1", "V1" },
                    { "K2", "V2" },
                    { "K3", "V3" },
                }
            };

            dynamicObject = new DynamicObject(source);
        }

        [Fact]
        public void Type_property_should_be_set_to_custom_class()
        {
            dynamicObject.Type.Type.ShouldBe(typeof(ClassWithDictionaryProperty));
        }

        [Fact]
        public void Should_have_a_single_member()
        {
            dynamicObject.MemberCount.ShouldBe(1);
        }

        [Fact]
        public void Member_name_should_be_name_of_property()
        {
            dynamicObject.MemberNames.Single().ShouldBe("Dictionary");
        }

        [Fact]
        public void Dictionary_property_should_be_object_array()
        {
            dynamicObject["Dictionary"].ShouldBeInstanceOf<object[]>();
        }

        [Fact]
        public void Dictionary_property_should_have_three_elements()
        {
            ((object[])dynamicObject["Dictionary"]).Length.ShouldBe(3);
        }

        [Fact]
        public void All_dictionary_elements_should_be_dynamic_objects()
        {
            foreach (var element in (object[])dynamicObject["Dictionary"])
            {
                element.ShouldBeInstanceOf<DynamicObject>();
            }
        }

        [Fact]
        public void All_dynamic_dictionary_elements_should_have_type_set_to_keyvaluepair()
        {
            foreach (DynamicObject element in (object[])dynamicObject["Dictionary"])
            {
                element.Type.Type.ShouldBe(typeof(KeyValuePair<string, string>));
            }
        }

        [Fact]
        public void All_dynamic_key_value_pairs_objects_should_have_two_members()
        {
            foreach (DynamicObject element in (object[])dynamicObject["Dictionary"])
            {
                element.MemberCount.ShouldBe(2);
            }
        }

        [Fact]
        public void All_dynamic_key_value_pairs_objects_should_have_key_and_value_member()
        {
            foreach (DynamicObject element in (object[])dynamicObject["Dictionary"])
            {
                element.MemberNames.ShouldContain("key");
                element.MemberNames.ShouldContain("value");
            }
        }

        [Fact]
        public void Dynamic_keys_and_values_should_match_source_values()
        {
            for (int i = 0; i < source.Dictionary.Count; i++)
            {
                DynamicObject element = (DynamicObject)((object[])dynamicObject["Dictionary"])[i];

                var key = source.Dictionary.Keys.ElementAt(i);
                var value = source.Dictionary.Values.ElementAt(i);

                element["key"].ShouldBe(key);
                element["value"].ShouldBe(value);
            }
        }
    }
}
