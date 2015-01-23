// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_created_based_on_datetime_object
    {
        DateTime value;
        DynamicObject dynamicObject;

        public When_created_based_on_datetime_object()
        {
            value = DateTime.Now;
            dynamicObject = new DynamicObject(value);
        }

        [Fact]
        public void Type_property_should_be_set_to_datetime()
        {
            dynamicObject.Type.Type.ShouldBe(typeof(DateTime));
        }

        [Fact]
        public void Should_have_a_single_member()
        {
            dynamicObject.MemberCount.ShouldBe(1);
        }

        [Fact]
        public void Member_name_should_be_empty_string()
        {
            dynamicObject.MemberNames.Single().ShouldBe(string.Empty);
        }

        [Fact]
        public void Member_value_should_be_initial_value()
        {
            dynamicObject[string.Empty].ShouldBe(value);
        }
    }
}
