// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common;

using Aqua.Protobuf;
using Remote.Linq.Protobuf;
using System.IO;

public static class ProtobufFormatter
{
    private static readonly ProtoOptions _options = ProtoOptions.WithRemoteLinqTypesOptimized with
    {
        DateTimeEncoding = DateTimeEncoding.Auto,
        TimeSpanEncoding = TimeSpanEncoding.Auto,
    };

    public static async ValueTask WriteAsync<T>(this Stream stream, T obj, CancellationToken cancellation = default)
    {
        byte[] data;
        byte datatype;
        if (obj is Exception exception)
        {
            datatype = 1;
            data = RemoteLinqProtobufSerializer.Serialize(exception.Message, _options);
        }
        else
        {
            datatype = 0;
            data = RemoteLinqProtobufSerializer.Serialize(obj, _options);
        }

        long size = data.LongLength;
        byte[] sizeData = BitConverter.GetBytes(size);

        stream.WriteByte(datatype);
        await stream.WriteAsync(sizeData, 0, sizeData.Length, cancellation).ConfigureAwait(false);
        await stream.WriteAsync(data, 0, data.Length, cancellation).ConfigureAwait(false);
    }

    public static async ValueTask<T> ReadAsync<T>(this Stream stream, CancellationToken cancellation = default)
    {
        int datatye = stream.ReadByte();
        if (datatye < 0)
        {
            throw new OperationCanceledException("Network stream was closed by other party.");
        }

        bool isException = datatye is 1;
        byte[] bytes = new byte[256];

        await stream.ReadAsync(bytes, 0, 8, cancellation).ConfigureAwait(false);
        long size = BitConverter.ToInt64(bytes, 0);

        using var dataStream = new MemoryStream();
        int count = 0;
        do
        {
            int length = size - count < bytes.Length
                ? (int)(size - count)
                : bytes.Length;

            int i = await stream.ReadAsync(bytes, 0, length, cancellation).ConfigureAwait(false);
            count += i;

            dataStream.Write(bytes, 0, i);
        }
        while (count < size);

        dataStream.Position = 0;
        if (isException)
        {
            var exceptionMessage = RemoteLinqProtobufSerializer.Deserialize<string>(dataStream, _options);
            throw new Exception(exceptionMessage);
        }

        var obj = RemoteLinqProtobufSerializer.Deserialize<T>(dataStream, _options);
        return obj;
    }
}
