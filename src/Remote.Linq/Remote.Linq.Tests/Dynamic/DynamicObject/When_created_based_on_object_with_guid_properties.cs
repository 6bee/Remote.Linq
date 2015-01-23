// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_object_with_guid_properties
    {
        class ClassWithGuidProperties
        {
            public Guid Guid1 { get; set; }
            public Guid? Guid2 { get; set; }
            public Guid? Guid3 { get; set; }
        }

        ClassWithGuidProperties source;
        DynamicObject dynamicObject;

        public When_created_based_on_object_with_guid_properties()
        {
            source = new ClassWithGuidProperties
            {
                Guid1 = Guid.NewGuid(),
                Guid2 = Guid.NewGuid(),
                Guid3 = null,
            };

            dynamicObject = new DynamicObject(source);
        }

        [Fact]
        public void Type_property_should_be_set_to_custom_class()
        {
            dynamicObject.Type.Type.ShouldBe(typeof(ClassWithGuidProperties));
        }

        [Fact]
        public void Should_have_a_single_member()
        {
            dynamicObject.MemberCount.ShouldBe(3);
        }

        [Fact]
        public void Member_name_should_be_name_of_property()
        {
            dynamicObject.MemberNames.ElementAt(0).ShouldBe("Guid1");
            dynamicObject.MemberNames.ElementAt(1).ShouldBe("Guid2");
            dynamicObject.MemberNames.ElementAt(2).ShouldBe("Guid3");
        }

        [Fact]
        public void Dynamic_guid_properties_should_be_of_type_string()
        {
            dynamicObject.Values.ElementAt(0).ShouldBeInstanceOf<Guid>();
            dynamicObject.Values.ElementAt(1).ShouldBeInstanceOf<Guid>();
            dynamicObject.Values.ElementAt(2).ShouldBeNull();
        }

        [Fact]
        public void Dynamic_guid_properties_should_be_string_represenation_of_guid_values()
        {
            dynamicObject["Guid1"].ShouldBe(source.Guid1);
            dynamicObject["Guid2"].ShouldBe(source.Guid2);
            dynamicObject["Guid3"].ShouldBeNull();
        }
    }
}
