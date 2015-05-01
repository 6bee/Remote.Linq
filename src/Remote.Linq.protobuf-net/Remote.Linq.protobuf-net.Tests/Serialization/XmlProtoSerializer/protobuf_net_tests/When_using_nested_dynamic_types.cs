// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.XmlProtoSerializer.protobuf_net_tests
{
    using ProtoBuf;
    using Xunit;
    using Xunit.Should;

    public class When_using_nested_dynamic_types
    {
        [ProtoContract]
        private class A
        {
            [ProtoMember(1, DynamicType = true)]
            public object Value { get; set; }
        }

        [ProtoContract]
        private class B
        {
            [ProtoMember(1, DynamicType = true)]
            public object Value { get; set; }
        }

        [ProtoContract]
        private class C
        {
            [ProtoMember(1)]
            public string Value { get; set; }
        }

        [ProtoContract]
        private class D
        {
            [ProtoMember(1)]
            public B Value { get; set; }
        }

        [Fact]
        public void Should_serialize_dynamic_type()
        {
            var obj = new A { Value = new C { Value = "Payload" } };

            var clone = Serializer.DeepClone(obj);

            (clone.Value as C).Value.ShouldBe("Payload");
        }

        [Fact]
        public void Should_serialize_nested_dynamic_type()
        {
            var obj = new A { Value = new A { Value = new C { Value = "Payload" } } };

            var clone = Serializer.DeepClone(obj);

            ((clone.Value as A).Value as C).Value.ShouldBe("Payload");
        }

        [Fact]
        public void Should_serialize_nested_dynamic_mixed_type()
        {
            var obj = new A { Value = new B { Value = new C { Value = "Payload" } } };

            var clone = Serializer.DeepClone(obj);

            ((clone.Value as B).Value as C).Value.ShouldBe("Payload");
        }

        [Fact]
        public void Should_serialize_dynamic_type_nested_in_hierarchy()
        {
            var obj = new D { Value = new B { Value = new C { Value = "Payload" } } };

            var clone = Serializer.DeepClone(obj);

            (clone.Value.Value as C).Value.ShouldBe("Payload");
        }

        [Fact]
        public void Should_serialize_multiple_dynamic_type_nested_in_hierarchy()
        {
            var obj = new D { Value = new B { Value = new D { Value = new B { Value = new C { Value = "Payload" } } } } };

            var clone = Serializer.DeepClone(obj);

            ((clone.Value.Value as D).Value.Value as C).Value.ShouldBe("Payload");
        }

        [Fact]
        public void Should_serialize_multiple_mixed_dynamic_type_nested_in_hierarchy()
        {
            var obj = new D { Value = new B { Value = new A { Value = new C { Value = "Payload" } } } };

            var clone = Serializer.DeepClone(obj);

            ((clone.Value.Value as A).Value as C).Value.ShouldBe("Payload");
        }
    }
}
