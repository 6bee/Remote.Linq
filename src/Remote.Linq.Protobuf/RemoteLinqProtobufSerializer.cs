// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf;

using Aqua.Protobuf;
using System.Buffers;
using System.IO;

/// <summary>
/// Schema-first Google.Protobuf serializer for <i>Remote Linq</i> types.
/// </summary>
public static class RemoteLinqProtobufSerializer
{
    /// <summary>
    /// Serializes the specified <paramref name="graph"/> to a protobuf-encoded byte array.
    /// </summary>
    /// <typeparam name="T">The type of the graph to serialize.</typeparam>
    /// <param name="graph">The object graph to serialize.</param>
    /// <param name="options">Protobuf serializer options.</param>
    /// <returns>The protobuf-encoded representation of <paramref name="graph"/>.</returns>
    public static byte[] Serialize<T>(T graph, ProtoOptions? options = null)
        => CreateWriteContext(options).Serialize(graph);

    /// <summary>
    /// Serializes the specified <paramref name="graph"/> to the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The type of the graph to serialize.</typeparam>
    /// <param name="graph">The object graph to serialize.</param>
    /// <param name="stream">The destination stream.</param>
    /// <param name="options">Protobuf serializer options.</param>
    public static void Serialize<T>(T graph, Stream stream, ProtoOptions? options = null)
        => CreateWriteContext(options).Serialize(graph, stream);

    /// <summary>
    /// Serializes the specified <paramref name="graph"/> to the given <paramref name="span"/>.
    /// </summary>
    /// <typeparam name="T">The type of the graph to serialize.</typeparam>
    /// <param name="graph">The object graph to serialize.</param>
    /// <param name="span">The destination span.</param>
    /// <param name="options">Protobuf serializer options.</param>
    public static void Serialize<T>(T graph, Span<byte> span, ProtoOptions? options = null)
        => CreateWriteContext(options).Serialize(graph, span);

    /// <summary>
    /// Serializes the specified <paramref name="graph"/> to the given <paramref name="writer"/>.
    /// </summary>
    /// <typeparam name="T">The type of the graph to serialize.</typeparam>
    /// <param name="graph">The object graph to serialize.</param>
    /// <param name="writer">The destination writer.</param>
    /// <param name="options">Protobuf serializer options.</param>
    public static void Serialize<T>(T graph, IBufferWriter<byte> writer, ProtoOptions? options = null)
        => CreateWriteContext(options).Serialize(graph, writer);

    /// <summary>
    /// Deserializes a graph of type <typeparamref name="T"/> from the protobuf-encoded <paramref name="data"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the deserialized graph.</typeparam>
    /// <param name="data">The protobuf-encoded representation to deserialize.</param>
    /// <param name="options">Protobuf serializer options.</param>
    /// <returns>The deserialized graph.</returns>
    public static T? Deserialize<T>(byte[] data, ProtoOptions? options = null)
    {
        data.AssertNotNull();
        return CreateReadContext(options).Deserialize<T>(data);
    }

    /// <summary>
    /// Deserializes a graph of type <typeparamref name="T"/> from the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the deserialized graph.</typeparam>
    /// <param name="stream">The source stream.</param>
    /// <param name="options">Protobuf serializer options.</param>
    /// <returns>The deserialized graph.</returns>
    public static T? Deserialize<T>(Stream stream, ProtoOptions? options = null)
    {
        stream.AssertNotNull();
        return CreateReadContext(options).Deserialize<T>(stream);
    }

    /// <summary>
    /// Deserializes a graph of type <typeparamref name="T"/> from the given <paramref name="data"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the deserialized graph.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="options">Protobuf serializer options.</param>
    /// <returns>The deserialized graph.</returns>
    public static T? Deserialize<T>(ReadOnlySequence<byte> data, ProtoOptions? options = null)
    {
        return CreateReadContext(options).Deserialize<T>(data);
    }

    /// <summary>
    /// Deserializes a graph of type <typeparamref name="T"/> from the given <paramref name="data"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the deserialized graph.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="options">Protobuf serializer options.</param>
    /// <returns>The deserialized graph.</returns>
    public static T? Deserialize<T>(ReadOnlySpan<byte> data, ProtoOptions? options = null)
    {
        return CreateReadContext(options).Deserialize<T>(data);
    }

    private static ProtoContext CreateReadContext(ProtoOptions? options) => ProtoContext.ForRead(options ?? ProtoOptions.WithRemoteLinqTypesOptimized);

    private static ProtoContext CreateWriteContext(ProtoOptions? options) => ProtoContext.ForWrite(options ?? ProtoOptions.WithRemoteLinqTypesOptimized);
}
