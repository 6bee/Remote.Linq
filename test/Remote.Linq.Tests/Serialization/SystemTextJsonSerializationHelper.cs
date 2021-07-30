// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Aqua;
    using Aqua.Text.Json.Converters;
    using System;
    using System.Numerics;
    using System.Text.Json;
    using Xunit;

    public static class SystemTextJsonSerializationHelper
    {
        private static readonly JsonSerializerOptions _serializerSettings = new JsonSerializerOptions { WriteIndented = true }
            .AddConverter(new TimeSpanConverter())
            .ConfigureRemoteLinq();

        public static T Serialize<T>(this T graph)
        {
            var json = JsonSerializer.Serialize(graph, _serializerSettings);

            return JsonSerializer.Deserialize<T>(json, _serializerSettings);
        }

        public static object Serialize(this object graph, Type type)
        {
            var json = JsonSerializer.Serialize(graph, _serializerSettings);

            return JsonSerializer.Deserialize(json, type, _serializerSettings);
        }

        public static void SkipUnsupportedDataType(Type type, object value)
        {
            Skip.If(type.Is<BigInteger>(), $"{type} not supported by out-of-the-box System.Text.Json");
            Skip.If(type.Is<Complex>(), $"{type} not supported by out-of-the-box System.Text.Json");
        }
    }
}