// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using global::MessagePack;

public static class MessagePackSerializationHelper
{
    public static T Clone<T>(this T graph)
    {
        var options = MessagePackSerializerOptions.Standard.ConfigureRemoteLinq();
        var data = MessagePackSerializer.Serialize(graph, options);
        return MessagePackSerializer.Deserialize<T>(data, options)!;
    }
}
