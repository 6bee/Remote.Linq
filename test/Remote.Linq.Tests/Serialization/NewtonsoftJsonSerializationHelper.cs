// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using global::Newtonsoft.Json;
    using Remote.Linq.Newtonsoft.Json;
    using System;

    public static class NewtonsoftJsonSerializationHelper
    {
        /// <summary>
        /// Gets pre-configured <see cref="JsonSerializerSettings"/> for <i>Aqua</i> types.
        /// </summary>
        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings { Formatting = Formatting.Indented }.ConfigureRemoteLinq();

        public static T Clone<T>(this T graph)
        {
            var json = JsonConvert.SerializeObject(graph, SerializerSettings);

            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        public static object Clone(this object graph, Type type)
        {
            var json = JsonConvert.SerializeObject(graph, SerializerSettings);

            return JsonConvert.DeserializeObject(json, type, SerializerSettings);
        }
    }
}