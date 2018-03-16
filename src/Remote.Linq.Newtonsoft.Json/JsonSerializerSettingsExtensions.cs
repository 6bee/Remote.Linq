// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Sets the <see cref="RemoteLinqContractResolver"/> in <see cref="JsonSerializerSettings"/>,
        /// decorating a previousely set <see cref="IContractResolver"/> if required.
        /// </summary>
        public static JsonSerializerSettings ConfigureRemoteLinq(this JsonSerializerSettings jsonSerializerSettings)
        {
            jsonSerializerSettings = jsonSerializerSettings.ConfigureAqua();

            jsonSerializerSettings.ContractResolver =
                jsonSerializerSettings.ContractResolver?.GetType() == typeof(DefaultContractResolver)
                    ? new RemoteLinqContractResolver()
                    : new RemoteLinqContractResolver(jsonSerializerSettings.ContractResolver);

            return jsonSerializerSettings;
        }
    }
}
