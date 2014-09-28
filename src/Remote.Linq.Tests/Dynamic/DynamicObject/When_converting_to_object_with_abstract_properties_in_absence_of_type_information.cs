// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_object_with_abstract_properties_in_absence_of_type_information
    {
        class CustomMapper : DynamicObjectMapper
        {
            protected override object MapFromDynamicObjectGraph(object obj, Type targetType)
            {
                if (targetType == typeof(BaseA))
                {
                    targetType = typeof(A);
                }
                return base.MapFromDynamicObjectGraph(obj, targetType);
            }
        }

        abstract class BaseA
        {   
        }

        class A : BaseA
        {
        }

        class ClassWithAbstractProperties
        {
            public BaseA Ref { get; set; }
            public object Value1 { get; set; }
            public object Value2 { get; set; }
            public object Value3 { get; set; }
            public object Value4 { get; set; }
        }

        DynamicObject dynamicObject;

        object obj;

        public When_converting_to_object_with_abstract_properties_in_absence_of_type_information()
        {
            dynamicObject = new DynamicObject()
            {
                { "Ref", new DynamicObject() },
                { "Value1", "the value's pay load" },
                { "Value2", 222 },
                { "Value3", null },
                { "Value4", new DynamicObject() },
            };

            var mapper = new CustomMapper();

            obj = mapper.Map<ClassWithAbstractProperties>(dynamicObject);
        }

        [Fact]
        public void Should_recreate_object_with_original_values()
        {
            obj.ShouldNotBeNull();
            obj.ShouldBeInstanceOf<ClassWithAbstractProperties>();

            var instance = (ClassWithAbstractProperties)obj;
            instance.Ref.ShouldBeInstanceOf<A>();
            instance.Value1.ShouldBe("the value's pay load");
            instance.Value2.ShouldBe(222);
            instance.Value3.ShouldBeNull();
            instance.Value4.ShouldBeInstanceOf<object>();
        }
    }
}
