// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf;

using Aqua.Protobuf;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ProtoOptionsExtensions
{
    extension(ProtoOptions)
    {
        /// <summary>
        /// Gets options with remote linq types registered.
        /// </summary>
        public static ProtoOptions WithRemoteLinqTypes => new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            Resolver = MapperResolver.WithRemoteLinqTypes,
        };

        /// <summary>
        /// Gets options with remote linq types registered with a performance optimized resolver,
        /// that uses shared static state to cache resolved instances for fast lookup.
        /// </summary>
        public static ProtoOptions WithRemoteLinqTypesOptimized => new()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            Resolver = MapperResolver.WithRemoteLinqTypes.Optimized(),
        };
    }
}
