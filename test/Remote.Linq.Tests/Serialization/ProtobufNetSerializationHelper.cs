// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using System;
    using System.Linq;
    using System.Numerics;
    using Xunit;
    using TypeModel = global::ProtoBuf.Meta.TypeModel;

    public static class ProtobufNetSerializationHelper
    {
        private static readonly TypeModel _configuration = CreateTypeModel();

        private static TypeModel CreateTypeModel()
        {
            var configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

            var testdatatypes = TestData.TestTypes
                .Select(x => (Type)x[0])
                .Select(x => x.AsNonNullableType())
                .Distinct()
                .ToArray();
            testdatatypes.ForEach(x => configuration.AddDynamicPropertyType(x));

            return configuration;
        }

        public static T Serialize<T>(this T graph) => Serialize(graph, null);

        public static T Serialize<T>(this T graph, TypeModel configuration)
        {
            configuration ??= _configuration;
            return (T)configuration.DeepClone(graph);
        }

        public static global::ProtoBuf.Meta.TypeModel CreateModelFor(Type type)
        {
            if (type.IsCollection())
            {
                type = TypeHelper.GetElementType(type);
            }

            if (type.IsEnum)
            {
                type = typeof(string);
            }

            return ProtoBufTypeModel.ConfigureRemoteLinq()
                .AddDynamicPropertyType(type)
                .Compile();
        }

        public static void SkipUnsupportedDataType(Type type, object value)
        {
            Skip.If(type.Is<DateTimeOffset>(), $"{type} not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<BigInteger>(), $"{type} not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<Complex>(), $"{type} not supported by out-of-the-box protobuf-net");
            Skip.If(type.IsNotPublic(), $"Not-public {type} not supported protobuf-net");
#if NET5_0
            Skip.If(type.Is<Half>(), $"{type} not supported by serializers");
#endif // NET5_0
        }
    }
}
