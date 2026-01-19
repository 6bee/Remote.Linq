// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json;

using Aqua.Newtonsoft.Json;
using global::Newtonsoft.Json;

public class RemoteLinqJsonSerializerSettings(JsonSerializerSettings settings, KnownTypesRegistry? knownTypesRegistry = null)
    : AquaJsonSerializerSettings(settings, knownTypesRegistry);