// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_converting_to_serializable_object
    {
        [Serializable]
        class SerializableType
        {
            public int Int32Value { get; set; }
            public string StringValue { get; set; }
        }

        DynamicObject dynamicObject;
        
        SerializableType obj;

        public When_converting_to_serializable_object()
        {
            dynamicObject = new DynamicObject()
            {
                { "Int32Value", 1 },
                { "StringValue", "one" },
            };

            obj = dynamicObject.CreateObject<SerializableType>();
        }

        [Fact]
        public void Should_converto_to_object_with_original_values()
        {
            obj.ShouldNotBeNull();
            obj.ShouldBeInstanceOf<SerializableType>();

            obj.Int32Value.ShouldBe(1);
            obj.StringValue.ShouldBe("one");
        }
    }
}
