// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObjectMapper
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_mapping_null
    {
        class CustomClass
        {
        }

        [Fact]
        public void Map_should_be_null_for_null_dynamic_object()
        {
            var result = new DynamicObjectMapper().Map((DynamicObject)null);
            result.ShouldBeNull();
        }

        [Fact]
        public void Map_type_should_be_null_for_null_dynamic_object()
        {
            var result = new DynamicObjectMapper().Map((DynamicObject)null, typeof(CustomClass));
            result.ShouldBeNull();
        }

        [Fact]
        public void Map_generic_type_should_be_null_for_null_dynamic_object()
        {
            var result = new DynamicObjectMapper().Map<CustomClass>((DynamicObject)null);
            result.ShouldBeNull();
        }

        [Fact]
        public void Map_type_should_throw_for_null_dynamic_object_enumerable()
        {
            Assert.Throws<ArgumentNullException>(() => new DynamicObjectMapper().Map((IEnumerable<DynamicObject>)null, typeof(object)));
        }

        [Fact]
        public void Map_generic_type_should_throw_for_null_dynamic_object_enumerable()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DynamicObjectMapper().Map<object>((IEnumerable<DynamicObject>)null));
        }

        [Fact]
        public void Map_generic_type_should_return_a_null_element_for_null_dynamic_object_enumerable_element()
        {
            var dynamicObjects = new DynamicObject[] 
            {
                new DynamicObject(new CustomClass()),
                null,
                new DynamicObject(new CustomClass()),
            };

            var result = new DynamicObjectMapper().Map<CustomClass>(dynamicObjects);
            
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.ElementAt(0).ShouldNotBeNull();
            result.ElementAt(1).ShouldBeNull();
            result.ElementAt(2).ShouldNotBeNull();
        }


        [Fact]
        public void Map_type_should_return_a_null_element_for_null_dynamic_object_enumerable_element()
        {
            var dynamicObjects = new DynamicObject[] 
            {
                new DynamicObject(new CustomClass()),
                null,
                new DynamicObject(new CustomClass()),
            };

            var result = new DynamicObjectMapper().Map(dynamicObjects, typeof(CustomClass)).Cast<object>();

            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.ElementAt(0).ShouldNotBeNull();
            result.ElementAt(1).ShouldBeNull();
            result.ElementAt(2).ShouldNotBeNull();
        }

        [Fact]
        public void Map_should_be_null_for_null_object()
        {
            var result = new DynamicObjectMapper().MapObject((object)null);
            result.ShouldBeNull();
        }

        [Fact]
        public void Map_single_should_be_null_for_null_object()
        {
            var result = new DynamicObjectMapper().MapObject((object)null);
            result.ShouldBeNull();
        }

        [Fact]
        public void Map_should_be_null_for_null_object_enumerable()
        {
            var objects = new object[] 
            {
                new CustomClass(),
                null,
                new CustomClass(),
            };

            var result = new DynamicObjectMapper().MapCollection(objects);

            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.ElementAt(0).ShouldNotBeNull();
            result.ElementAt(1).ShouldBeNull();
            result.ElementAt(2).ShouldNotBeNull();
        }
    }
}
