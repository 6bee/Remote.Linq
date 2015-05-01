// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.XmlProtoSerializer.protobuf_net_tests
{
    using ProtoBuf.Meta;
    using Remote.Linq.Dynamic;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Xunit;
    using Xunit.Should;

    public class When_using_danymic_object
    {
        private readonly TypeModel _serializer;

        public When_using_danymic_object()
        {
            var config = TypeModel.Create();

            var containerType = config[typeof(DynamicObject)];

            var typeProperty = containerType.GetFields().Single(x => string.Equals(x.Name, "Type"));
            typeProperty.SupportNull = true;

            var propertyType = config[typeof(DynamicObject.Property)];

            var valueProperty = propertyType.GetFields().Single(x => string.Equals(x.Name, "Value"));
            valueProperty.DynamicType = true;
            valueProperty.AsReference = true; // required for nested container to work

            _serializer = config.Compile();
        }

        [Fact]
        public void Should_serialize_nested_container_with_dynamic_property()
        {
            var nestedContainer = new DynamicObject();

            var valueProperty = new DynamicObject.Property { Name = "Text", Value = "String content" };

            nestedContainer.Members = new List<DynamicObject.Property>() { valueProperty };

            var container = new DynamicObject();

            var containerProperty = new DynamicObject.Property { Name = "Nested Container", Value = nestedContainer };

            container.Members = new List<DynamicObject.Property>() { containerProperty };


            var clone = _serializer.DeepClone(container) as DynamicObject;


            var innerContainerClone = clone.Members.Single(x => x.Name == "Nested Container").Value as DynamicObject;

            innerContainerClone.Members.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }

        [Fact]
        public void Should_serialize_container_with_dynamic_property()
        {
            var container = new DynamicObject();

            container.Members = new List<DynamicObject.Property>() { new DynamicObject.Property { Name = "Text", Value = "String content" } };


            var clone = _serializer.DeepClone(container) as DynamicObject;


            clone.Members.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }
    }
}
