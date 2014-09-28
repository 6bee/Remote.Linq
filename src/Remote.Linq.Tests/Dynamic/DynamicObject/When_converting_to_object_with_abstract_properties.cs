// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_object_with_abstract_properties
    {
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

        public When_converting_to_object_with_abstract_properties()
        {
            dynamicObject = new DynamicObject()
            {
                { "Ref", new DynamicObject(typeof(A)) },
                { "Value1", "the value's pay load" },
                { "Value2", 222 },
                { "Value3", null },
                { "Value4", new DynamicObject(typeof(object))},
            };

            obj = dynamicObject.CreateObject<ClassWithAbstractProperties>();
        }

        [Fact]
        public void Should_converto_to_object_with_original_values()
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
