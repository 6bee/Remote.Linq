// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_object_with_dictionary_property
    {
        class ClassWithDictionaryProperty
        {
            public IDictionary<string, string> Dictionary { get; set; }
        }

        DynamicObject dynamicObject;
        object obj;
        ClassWithDictionaryProperty objectWithDictionaryProperty;

        public When_converting_to_object_with_dictionary_property()
        {
            dynamicObject = new DynamicObject(typeof(ClassWithDictionaryProperty))
            {
                { 
                    "Dictionary", 
                    new object[]
                    {
                        new KeyValuePair<string,string>("K1", "V1"),
                        new KeyValuePair<string,string>("K2", "V2"),
                        new KeyValuePair<string,string>("K3", "V3"),
                    }
                },
            };

            obj = DynamicObjectMapper.InstanceProvider().Map(dynamicObject);

            objectWithDictionaryProperty = obj as ClassWithDictionaryProperty;
        }

        [Fact]
        public void Object_should_not_be_null()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Object_should_be_of_expected_type()
        {
            obj.ShouldBeInstanceOf<ClassWithDictionaryProperty>();
            objectWithDictionaryProperty.ShouldNotBeNull();
        }

        [Fact]
        public void Object_should_have_dictionary_proeprty_set()
        {
            objectWithDictionaryProperty.Dictionary.ShouldNotBeNull();
        }

        [Fact]
        public void Dictionary_should_contain_extected_number_of_elements()
        {
            var expectedNumberOfElements = ((object[])dynamicObject["Dictionary"]).Length;
            objectWithDictionaryProperty.Dictionary.Count.ShouldBe(expectedNumberOfElements);
        }

        [Fact]
        public void Dictionary_elements_should_contain_dynamic_object_values()
        {
            var elements = ((object[])dynamicObject["Dictionary"]).Cast<KeyValuePair<string,string>>().ToList();
            for(int i = 0; i < elements.Count; i++)
            {
                var key = elements[i].Key;
                var value = elements[i].Value;

                objectWithDictionaryProperty.Dictionary[key].ShouldBe(value);
            };
        }
    }
}
