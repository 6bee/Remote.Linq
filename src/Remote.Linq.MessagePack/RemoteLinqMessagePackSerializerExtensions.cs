// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MessagePack;
#pragma warning restore IDE0130

using Aqua.MessagePack;
using Remote.Linq.MessagePack;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteLinqMessagePackSerializerExtensions
{
    extension(MessagePackSerializerOptions options)
    {
        /// <summary>
        /// Returns a copy of the <see cref="MessagePackSerializerOptions"/> configured to serialize
        /// Remote.Linq and Aqua types using type-safe formatters, with reference preservation enabled.
        /// </summary>
        public AquaMessagePackSerializerOptions ConfigureRemoteLinq()
        {
            options.AssertNotNull();
            var resolver = new RemoteLinqFormatterResolver(new AquaFormatterResolver(options.Resolver));
            var withResolver = options.WithResolver(resolver);
            return (withResolver as AquaMessagePackSerializerOptions ?? new AquaMessagePackSerializerOptions(withResolver))
                .WithPreserveReferences();
        }
    }
}
