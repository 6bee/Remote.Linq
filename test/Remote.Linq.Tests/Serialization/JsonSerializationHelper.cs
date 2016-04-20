// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Newtonsoft.Json;

    public static class JsonSerializationHelper
    {
        static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public static T Serialize<T>(this T graph)
        {
            var json = JsonConvert.SerializeObject(graph, _serializerSettings);

            return JsonConvert.DeserializeObject<T>(json, _serializerSettings);
        }
    }
}
