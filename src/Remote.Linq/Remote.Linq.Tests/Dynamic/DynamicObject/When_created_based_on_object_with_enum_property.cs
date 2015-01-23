// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_object_with_enum_property
    {
        enum CustomEnum
        {
            Value1,
            Value2,
            Value3,
        }

        class ClassWithEnum
        {
            public CustomEnum EnumProperty { get; set; }
        }

        ClassWithEnum source;
        DynamicObject dynamicObject;

        public When_created_based_on_object_with_enum_property()
        {
            source = new ClassWithEnum
            {
                EnumProperty = CustomEnum.Value2
            };

            dynamicObject = new DynamicObject(source);
        }

        [Fact]
        public void Member_count_should_be_one()
        {
            dynamicObject.MemberCount.ShouldBe(1);
        }

        [Fact]
        public void Member_name_should_be_name_of_property()
        {
            dynamicObject.MemberNames.ShouldContain("EnumProperty");
        }

        [Fact]
        public void Member_value_should_be_stringvalue_of_enum_property()
        {
            var enumValueString = source.EnumProperty.ToString();
            dynamicObject["EnumProperty"].ShouldBe(enumValueString);
        }

        [Fact]
        public void Type_property_should_be_set_to_type_of_source_object()
        {
            dynamicObject.Type.ShouldNotBeNull();
            dynamicObject.Type.Type.ShouldBe(typeof(ClassWithEnum));
        }
    }
}
