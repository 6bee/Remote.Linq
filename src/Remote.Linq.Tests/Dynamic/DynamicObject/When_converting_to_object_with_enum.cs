// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_object_with_enum
    {
        enum CustomEnum
        {
            Value0 = 0,
            Value1 = 1,
            Value2 = 2,
            Value3 = 3,
        }

        class ClassWithEnum
        {
            public CustomEnum EnumProperty { get; set; }
        }

        DynamicObject[] dynamicObjects;
        IEnumerable<ClassWithEnum> objects;

        public When_converting_to_object_with_enum()
        {
            dynamicObjects = new[]
            {
                new DynamicObject(typeof(ClassWithEnum))
                {
                    { "EnumProperty", CustomEnum.Value1 }
                },
                new DynamicObject(typeof(ClassWithEnum))
                {
                    { "EnumProperty", CustomEnum.Value2.ToString().ToUpper() }
                },
                new DynamicObject(typeof(ClassWithEnum))
                {
                    { "EnumProperty", (int)CustomEnum.Value3 }
                },
            };

            objects = DynamicObjectMapper.Map<ClassWithEnum>(dynamicObjects);
        }

        [Fact]
        public void Number_of_obects_should_be_number_of_dynamic_objects()
        {
            objects.Count().ShouldBe(dynamicObjects.Length);
        }

        [Fact]
        public void Enum_property_should_be_set_according_enum_value()
        {
            objects.ElementAt(0).EnumProperty.ShouldBe(CustomEnum.Value1);
        }

        [Fact]
        public void Enum_property_should_be_set_according_string_value()
        {
            objects.ElementAt(1).EnumProperty.ShouldBe(CustomEnum.Value2);
        }

        [Fact]
        public void Enum_property_should_be_set_according_int_value()
        {
            objects.ElementAt(2).EnumProperty.ShouldBe(CustomEnum.Value3);
        }
    }
}
