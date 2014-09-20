// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Should;

    public class When_converting_to_object_with_guid_properties
    {
        class ClassWithGuidProperties
        {
            public Guid Guid1 { get; set; }
            public Guid? Guid2 { get; set; }
            public Guid? Guid3 { get; set; }
        }

        Guid guid1;
        Guid guid2;
        ClassWithGuidProperties obj;

        public When_converting_to_object_with_guid_properties()
        {
            const string guid1String = "664f96e9-80e2-41ee-9799-0403d4338fba";
            const string guid2String = "e9ad609f-deca-45d2-8478-d4e0768dda40";

            guid1 = new Guid(guid1String);
            guid2 = new Guid(guid2String);

            var dynamicObject = new DynamicObject(typeof(ClassWithGuidProperties))
            {
                { "Guid1", guid1String },
                { "Guid2", guid2String },
                { "Guid3", null },
            }; ;

            obj = DynamicObjectMapper.Map(dynamicObject) as ClassWithGuidProperties;
        }

        [Fact]
        public void Should_create_on_instance()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_have_a_single_member()
        {
            obj.Guid1.ShouldBe(guid1);
            obj.Guid2.ShouldBe(guid2);
            obj.Guid3.ShouldBeNull();
        }
    }
}
