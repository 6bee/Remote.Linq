// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_custom_type
    {
        class CustomClass
        {
            public string Prop1 { get; set; }
        }

        CustomClass source;
        DynamicObject dynamicObject;

        public When_created_based_on_custom_type()
        {
            source = new CustomClass { Prop1 = "Value1" };
            dynamicObject = new DynamicObject(source);
        }

        [Fact]
        public void Type_property_should_be_set_to_custom_class()
        {
            dynamicObject.Type.Type.ShouldBe(typeof(CustomClass));
        }

        [Fact]
        public void Should_have_a_single_member()
        {
            dynamicObject.MemberCount.ShouldBe(1);
        }

        [Fact]
        public void Member_name_should_be_name_of_property()
        {
            dynamicObject.MemberNames.Single().ShouldBe("Prop1");
        }

        [Fact]
        public void Member_value_should_be_property_value()
        {
            dynamicObject["Prop1"].ShouldBe(source.Prop1);
        }
    }
}
