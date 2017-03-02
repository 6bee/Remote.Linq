// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Sets the <see cref="AquaContractResolver"/> in <see cref="JsonSerializerSettings"/>, 
        /// decorating a previousely set <see cref="IContractResolver"/> if required.
        /// </summary>
        public static JsonSerializerSettings ConfigureAqua(this JsonSerializerSettings jsonSerializerSettings)
        {
            var valid = new[] { TypeNameHandling.All, TypeNameHandling.Auto, TypeNameHandling.Objects };
            if (!valid.Contains(jsonSerializerSettings.TypeNameHandling))
            {
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            }

            jsonSerializerSettings.ContractResolver = jsonSerializerSettings.ContractResolver?.GetType() == typeof(DefaultContractResolver)
                ? new RemoteLinqContractResolver()
                : new RemoteLinqContractResolver(jsonSerializerSettings.ContractResolver);

            return jsonSerializerSettings;
        }
    }
}
