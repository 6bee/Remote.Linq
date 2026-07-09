// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Newtonsoft.Json;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Aqua;

public class RemoteLinqJsonSerializerSettings(JsonSerializerSettings settings, KnownTypesRegistry? knownTypesRegistry = null)
    : AquaJsonSerializerSettings(settings, knownTypesRegistry);
