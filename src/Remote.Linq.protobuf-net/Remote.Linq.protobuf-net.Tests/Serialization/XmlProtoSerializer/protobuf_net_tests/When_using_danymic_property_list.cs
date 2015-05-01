// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.XmlProtoSerializer.protobuf_net_tests
{
    using ProtoBuf;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Should;

    public class When_using_danymic_property_list
    {
        [ProtoContract]
        public class DataContainer
        {
            public DataContainer()
            {
                Properties = new List<Property>();
            }

            [ProtoMember(1)]
            public List<Property> Properties { get; set; }
        }

        [ProtoContract]
        public class Property
        {
            [ProtoMember(1)]
            public string Name { get; set; }

            [ProtoMember(2, DynamicType = true, AsReference = true)]
            public object Value { get; set; }
        }

        [Fact]
        public void Should_serialize_nested_container_with_dynamic_property()
        {
            var nestedContainer = new DataContainer();

            var valueProperty = new Property { Name = "Text", Value = "String content" };

            nestedContainer.Properties.Add(valueProperty);

            var container = new DataContainer();

            var containerProperty = new Property { Name = "Nested Container", Value = nestedContainer };

            container.Properties.Add(containerProperty);

            var clone = Serializer.DeepClone(container);

            var innerContainerClone = clone.Properties.Single(x => x.Name == "Nested Container").Value as DataContainer;

            innerContainerClone.Properties.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }

        [Fact]
        public void Should_serialize_with_dynamic_property()
        {
            var container = new DataContainer();

            container.Properties.Add(new Property { Name = "Text", Value = "String content" });

            var clone = Serializer.DeepClone(container);

            clone.Properties.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }
    }
}
