// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.Protobuf;
using Remote.Linq.Protobuf;

public static class ProtobufSerializationHelper
{
    private static readonly ProtoOptions _options = ProtoOptions.WithRemoteLinqTypesOptimized with
    {
        DateTimeEncoding = DateTimeEncoding.Auto,
        TimeSpanEncoding = TimeSpanEncoding.Auto,
    };

    public static T Clone<T>(this T graph)
    {
        var data = RemoteLinqProtobufSerializer.Serialize(graph, _options);
        var copy = RemoteLinqProtobufSerializer.Deserialize<T>(data, _options);
        return copy;
    }
}
