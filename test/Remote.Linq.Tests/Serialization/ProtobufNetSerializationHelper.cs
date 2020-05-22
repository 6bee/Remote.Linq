// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Numerics;
    using Xunit;

    public static class ProtobufNetSerializationHelper
    {
#if COREFX
        private static readonly global::ProtoBuf.Meta.RuntimeTypeModel _configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

        public static T Serialize<T>(this T graph) => Serialize(graph, null);

        public static T Serialize<T>(this T graph, global::ProtoBuf.Meta.RuntimeTypeModel configuration)
        {
            configuration ??= _configuration;
            return (T)configuration.DeepClone(graph);
        }
#endif // COREFX

        public static void SkipUnsupportedDataType(Type type, object value)
        {
            Skip.If(type.Is<DateTimeOffset>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<BigInteger>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(type.Is<Complex>(), "Data type not supported by out-of-the-box protobuf-net");
            Skip.If(type.IsCollection() && ((IEnumerable)value).Cast<object>().All(x => x is null), "Collections with only null elements are set null by out-of-the-box protobuf-net");
        }
    }
}
