// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Numerics;
    using Xunit;

    public static class ProtobufNetSerializationHelper
    {
#if NETCOREAPP
        private static readonly global::ProtoBuf.Meta.TypeModel _configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

        public static T Serialize<T>(this T graph) => Serialize(graph, null);

        public static T Serialize<T>(this T graph, global::ProtoBuf.Meta.TypeModel configuration)
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
#endif // NETCOREAPP

        public static void SkipUnsupportedDataType(Type type, object value)
        {
            Skip.If(type.Is<DateTimeOffset>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<BigInteger>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<Complex>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(
                type.IsCollection() && ((IEnumerable)value).Cast<object>().Any(x => x is null),
                "protobuf-net doesn't support serialization of collection with null elements as the root object");
        }
    }
}
