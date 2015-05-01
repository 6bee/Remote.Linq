// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.XmlProtoSerializer.protobuf_net_tests
{
    using ProtoBuf.Meta;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Xunit;
    using Xunit.Should;

    public class When_using_danymic_property_list_with_separate_proto_config
    {
        [Serializable]
        [DataContract(IsReference = true)]
        public class DataContainer
        {
            [Serializable]
            [DataContract(IsReference = true)]
            public class Property
            {
                [DataMember(Order = 1)]
                public string Name { get; set; }

                [DataMember(Order = 2)]
                public object Value { get; set; }
            }

            private Dictionary<string, object> _data = new Dictionary<string, object>();

            public DataContainer()
            {
                Properties = new List<Property>();
            }

            [DataMember(Order = 1)]
            //public IEnumerable<Property> Properties { get; set; }
            public IEnumerable<Property> Properties
            {
                get
                {
                    return _data.Select(x => new Property { Name = x.Key, Value = x.Value }).ToList();
                }
                set
                {
                    _data = value.ToDictionary(x => x.Name, x => x.Value);
                }
            }
        }

        private readonly TypeModel _serializer;

        public When_using_danymic_property_list_with_separate_proto_config()
        {
            var config = TypeModel.Create();

            var containerType = config[typeof(DataContainer)];

            var propertiesProperty = containerType.GetFields().Single(x => string.Equals(x.Name, "Properties"));
            propertiesProperty.OverwriteList = true;

            var propertyType = config[typeof(DataContainer.Property)];

            var valueProperty = propertyType.GetFields().Single(x => string.Equals(x.Name, "Value"));
            valueProperty.DynamicType = true;
            valueProperty.AsReference = true; // required for nested container to work

            _serializer = config.Compile();
        }

        [Fact]
        public void Should_serialize_nested_container_with_dynamic_property()
        {
            var nestedContainer = new DataContainer();

            var valueProperty = new DataContainer.Property { Name = "Text", Value = "String content" };

            nestedContainer.Properties = new[] { valueProperty };

            var container = new DataContainer();

            var containerProperty = new DataContainer.Property { Name = "Nested Container", Value = nestedContainer };

            container.Properties = new[] { containerProperty };


            var clone = _serializer.DeepClone(container) as DataContainer;


            var innerContainerClone = clone.Properties.Single(x => x.Name == "Nested Container").Value as DataContainer;

            innerContainerClone.Properties.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }

        [Fact]
        public void Should_serialize_container_with_dynamic_property()
        {
            var container = new DataContainer();

            container.Properties = new[] { new DataContainer.Property { Name = "Text", Value = "String content" } };


            var clone = _serializer.DeepClone(container) as DataContainer;


            clone.Properties.Single(x => x.Name == "Text").Value.ShouldBe("String content");
        }
    }
}
