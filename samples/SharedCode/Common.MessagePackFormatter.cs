// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common;

using MessagePack;
using System.IO;

public static class MessagePackFormatter
{
    private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.ConfigureRemoteLinq();

    /// <summary>
    /// Write serialized object to stream using MessagePack serializer.
    /// </summary>
    public static async ValueTask WriteAsync<T>(this Stream stream, T obj, CancellationToken cancellation = default)
    {
        byte[] bin;
        byte datatype;
        if (obj is Exception exception)
        {
            datatype = 1;
            bin = MessagePackSerializer.Serialize(exception.Message, _options, cancellation);
        }
        else
        {
            datatype = 0;
            bin = MessagePackSerializer.Serialize(obj, _options, cancellation);
        }

        stream.WriteByte(datatype);
        var len = BitConverter.GetBytes(bin.Length);
        stream.WriteByte((byte)len.Length);
        await stream.WriteAsync(len, cancellation).ConfigureAwait(false);
        await stream.WriteAsync(bin, cancellation).ConfigureAwait(false);
    }

    /// <summary>
    /// Read and deserialze object from stream using MessagePack serializer.
    /// </summary>
    public static async ValueTask<T> ReadAsync<T>(this Stream stream, CancellationToken cancellation = default)
    {
        var t = stream.ReadByte();
        bool isException = t is 1;

        var c = stream.ReadByte();
        var len = new byte[c];
        if (await stream.ReadAsync(len, 0, len.Length, cancellation).ConfigureAwait(false) != len.Length)
        {
            throw new IOException("Failed to read data");
        }

        var messageSize = BitConverter.ToInt32(len);
        if (messageSize is 0)
        {
            throw new IOException("Unexpected empty message");
        }

        var bin = new byte[messageSize];
        var buffer = new byte[512];
        var count = 0;
        do
        {
            var length = Math.Min(buffer.Length, messageSize - count);
            var size = await stream.ReadAsync(buffer, 0, length, cancellation).ConfigureAwait(false);
            Array.Copy(buffer, 0, bin, count, size);
            count += size;
        }
        while (count < messageSize);

        if (isException)
        {
            var exceptionMessage = MessagePackSerializer.Deserialize<string>(bin, _options, cancellation);
            throw new Exception(exceptionMessage);
        }

        var obj = MessagePackSerializer.Deserialize<T>(bin, _options, cancellation);
        return obj;
    }
}
