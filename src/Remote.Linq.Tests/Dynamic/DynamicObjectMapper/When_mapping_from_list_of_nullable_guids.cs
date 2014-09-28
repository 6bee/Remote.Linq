// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_from_list_of_nullable_guids
    {
        List<Guid?> source;
        IEnumerable<DynamicObject> dynamicObjects;

        public When_mapping_from_list_of_nullable_guids()
        {
            source = new List<Guid?> { Guid.NewGuid(), Guid.NewGuid(), null };
            dynamicObjects = new DynamicObjectMapper().MapCollection(source);
        }

        [Fact]
        public void Dynamic_objects_count_should_be_three()
        {
            dynamicObjects.Count().ShouldBe(3);
        }

        [Fact]
        public void Dynamic_objects_type_property_should_be_set_to_guid()
        {
            dynamicObjects.ElementAt(0).Type.Type.ShouldBe(typeof(Guid));
            dynamicObjects.ElementAt(1).Type.Type.ShouldBe(typeof(Guid));
            dynamicObjects.ElementAt(2).ShouldBeNull();
        }

        [Fact]
        public void Dynamic_objects_should_have_one_member()
        {
            dynamicObjects.ElementAt(0).MemberCount.ShouldBe(1);
            dynamicObjects.ElementAt(1).MemberCount.ShouldBe(1);
            dynamicObjects.ElementAt(2).ShouldBeNull();
        }

        [Fact]
        public void Dynamic_objects_member_name_should_be_empty_string()
        {
            dynamicObjects.ElementAt(0).MemberNames.Single().ShouldBe(string.Empty);
            dynamicObjects.ElementAt(1).MemberNames.Single().ShouldBe(string.Empty);
            dynamicObjects.ElementAt(2).ShouldBeNull();
        }

        [Fact]
        public void Dynamic_objects_member_values_should_be_value_of_source()
        {
            dynamicObjects.ElementAt(0)[string.Empty].ShouldBe(source[0]);
            dynamicObjects.ElementAt(1)[string.Empty].ShouldBe(source[1]);
            dynamicObjects.ElementAt(2).ShouldBeNull();
        }
    }
}
