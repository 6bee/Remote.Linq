// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.XmlProtoSerializer.protobuf_net_tests
{
    using ProtoBuf;
    using Xunit;
    using Xunit.Should;

    public class When_using_danymic_property
    {
        [ProtoContract]
        public class DataContainer
        {
            [ProtoMember(1, DynamicType = true/*, AsReference = true*/)]
            public object Value { get; set; }
        }

        [ProtoContract]
        public class DataItem
        {
            [ProtoMember(1)]
            public double Number { get; set; }

            [ProtoMember(2)]
            public string Text { get; set; }
        }

        [Fact]
        public void Should_serialize_with_numeric_type()
        {
            var container = new DataContainer { Value = 9.8765432e12 };

            // System.InvalidOperationException : Dynamic type is not a contract-type: Double
            var clone = Serializer.DeepClone(container);

            clone.Value.ShouldBe(9.8765432e12);
        }

        [Fact]
        public void Should_serialize_with_nested_container_type()
        {
            var container = new DataContainer { Value = new DataContainer { Value = "String content" } };

            // ProtoBuf.ProtoException : Internal error; a key mismatch occurred
            // Note: this seems to be resolved when setting [ProtoMember(... ,AsReference=true)]
            var clone = Serializer.DeepClone(container);

            clone.Value.ShouldBeInstanceOf<DataContainer>();

            var nestedContainer = (DataContainer)clone.Value;
            nestedContainer.Value.ShouldBe("String content");
        }

        [Fact]
        public void Should_serialize_with_string_type()
        {
            var container = new DataContainer { Value = "String content" };

            var clone = Serializer.DeepClone(container);

            clone.Value.ShouldBe("String content");
        }

        [Fact]
        public void Should_serialize_with_reference_type()
        {
            var container = new DataContainer { Value = new DataItem { Number = 9.8765432e12, Text = "String content" } };

            var clone = Serializer.DeepClone(container);

            clone.Value.ShouldBeInstanceOf<DataItem>();

            var item = (DataItem)clone.Value;
            item.Number.ShouldBe(9.8765432e12);
            item.Text.ShouldBe("String content");
        }
    }
}
