// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ProtoBuf;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.ProtoBuf;
using Shouldly;
using System;
using System.Collections;
using System.Linq;
using Xunit;
using static Remote.Linq.Tests.Serialization.ProtobufNetSerializationHelper;

/// <summary>
/// This class is rather a documentation of know limitations to <i>Remote.Linq</i>'s support for <i>protobuf-net</i> that actual tests.
/// </summary>
public class When_serializing_remote_linq_types_with_protobuf_net
{
    [SkippableTheory]
    [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueArrays), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueLists), MemberType = typeof(TestData))]
    public void Should_serialize_property(Type type, object value)
    {
        SkipUnsupportedDataType(type, value);

        var property = new Property("p1", value);

        var config = ProtoBufTypeModel.ConfigureRemoteLinq(configureDefaultSystemTypes: false)
            .AddDynamicPropertyType(TypeHelper.GetElementType(type))
            .Compile();
        var copy = property.Clone(config);

        copy.Value.ShouldBe(value);
    }

    [Fact]
    public void Should_serialize_property_set()
    {
        var propertySet = new PropertySet
        {
            { "p1", "v1" },
            { "p2", null },
            { "p3", 1 },
        };

        var copy = propertySet.Clone();

        copy["p1"].ShouldBe(propertySet["p1"]);
        copy["p2"].ShouldBe(propertySet["p2"]);
        copy["p3"].ShouldBe(propertySet["p3"]);
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueArrays), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueLists), MemberType = typeof(TestData))]
    public void Should_serialize_property_set_2(Type type, object value)
    {
        SkipUnsupportedDataType(type, value);

        var propertySet = new PropertySet
        {
            { "p1", value },
        };

        var config = ProtoBufTypeModel.ConfigureRemoteLinq(configureDefaultSystemTypes: false)
            .AddDynamicPropertyType(TypeHelper.GetElementType(type))
            .Compile();
        var copy = propertySet.Clone(config);

        copy["p1"].ShouldBe(value);
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
    public void Should_serialize_dynamic_object(Type type, object value)
    {
        SkipUnsupportedDataType(type, value);

        var dynamicObject = new DynamicObjectMapper().MapObject(value);

        var config = CreateModelFor(type);
        var copy = dynamicObject.Clone(config);

        copy?.Get().ShouldBe(dynamicObject?.Get(), $"type: {type} value: {value}");

        var c = new DynamicObjectMapper().Map(copy);
        c.ShouldBe(value);
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValueArrays), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueLists), MemberType = typeof(TestData))]
    public void Should_serialize_dynamic_object_collection(Type type, IEnumerable value)
    {
        SkipUnsupportedDataType(type, value);

        var dynamicObjects = new DynamicObjectMapper(new DynamicObjectMapperSettings { WrapNullAsDynamicObject = true }).MapCollection(value);

        var config = CreateModelFor(type);
        var copy = dynamicObjects.Clone(config);

        var dynamicObjectsCount = dynamicObjects?.Count ?? 0;
        var copyCount = copy?.Count ?? 0;
        copyCount.ShouldBe(dynamicObjectsCount);

        for (int i = 0; i < copyCount; i++)
        {
            var originalValue = dynamicObjects.ElementAt(i)?.Get();
            var copyValue = copy.ElementAt(i)?.Get();
            copyValue.ShouldBe(originalValue);
        }

        var c = new DynamicObjectMapper().Map(copy);
        if (value is null)
        {
            c.ShouldBeNull();
        }
        else
        {
            var array = value.Cast<object>().ToArray();
            var copyArray = c.Cast<object>().ToArray();

            for (int i = 0; i < copyArray.Length; i++)
            {
                copyArray[i].ShouldBe(array[i]);
            }
        }
    }
}