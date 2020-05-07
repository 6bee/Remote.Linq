// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Serialization;
    using Remote.Linq.Newtonsoft.Json.ContractResolvers;
    using System;
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

            if (jsonSerializerSettings.ContractResolver?.GetType() != typeof(RemoteLinqContractResolver))
            {
                jsonSerializerSettings.ContractResolver = new RemoteLinqContractResolver(jsonSerializerSettings.ContractResolver);
            }

            return jsonSerializerSettings;
        }
    }
}
