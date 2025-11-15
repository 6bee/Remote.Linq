// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinaryDataFormatter
{
    public static void Write(this Stream stream, object obj)
    {
        byte[] data;
        using (var dataStream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(dataStream, obj);
            dataStream.Position = 0;
            data = dataStream.ToArray();
        }

        long size = data.LongLength;
        byte[] sizeData = BitConverter.GetBytes(size);

        stream.Write(sizeData, 0, sizeData.Length);
        stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
        stream.Write(data, 0, data.Length);
    }

    public static T Read<T>(this Stream stream)
    {
        byte[] bytes = new byte[256];

        if (stream.Read(bytes, 0, 8) != 8)
        {
            throw new InvalidOperationException("Could not read data from stream!");
        }

        long size = BitConverter.ToInt64(bytes, 0);

        bool isException = stream.ReadByte() != 0;

        object obj;
        using (var dataStream = new MemoryStream())
        {
            int count = 0;
            do
            {
                int length = size - count < bytes.Length
                    ? (int)(size - count)
                    : bytes.Length;

                int i = stream.Read(bytes, 0, length);
                count += i;

                dataStream.Write(bytes, 0, i);
            }
            while (count < size);

            dataStream.Position = 0;

            var formatter = new BinaryFormatter();
            obj = formatter.Deserialize(dataStream);
        }

        if (isException)
        {
            var exception = (Exception)obj;
            throw exception;
        }

        return (T)obj;
    }

    public static async ValueTask WriteAsync(this Stream stream, object obj, CancellationToken cancellation = default)
    {
        byte[] data;
        using (var dataStream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(dataStream, obj);
            dataStream.Position = 0;
            data = dataStream.ToArray();
        }

        long size = data.LongLength;
        byte[] sizeData = BitConverter.GetBytes(size);

        await stream.WriteAsync(sizeData, 0, sizeData.Length, cancellation).ConfigureAwait(false);
        await stream.WriteAsync([obj is Exception ? (byte)1 : (byte)0], 0, 1, cancellation).ConfigureAwait(false);
        await stream.WriteAsync(data, 0, data.Length, cancellation).ConfigureAwait(false);
    }

    public static async ValueTask<T> ReadAsync<T>(this Stream stream, CancellationToken cancellation = default)
    {
        byte[] bytes = new byte[256];

        await stream.ReadAsync(bytes, 0, 8, cancellation).ConfigureAwait(false);
        long size = BitConverter.ToInt64(bytes, 0);

        byte[] exceptionFlag = new byte[1];
        int i = await stream.ReadAsync(exceptionFlag, 0, 1, cancellation).ConfigureAwait(false);
        if (i != 1)
        {
            throw new IOException("Unable to read expected error indication flag.");
        }

        object obj;
        using (var dataStream = new MemoryStream())
        {
            int count = 0;
            do
            {
                int length = size - count < bytes.Length
                    ? (int)(size - count)
                    : bytes.Length;

                i = await stream.ReadAsync(bytes, 0, length, cancellation).ConfigureAwait(false);
                count += i;

                await dataStream.WriteAsync(bytes, 0, i, cancellation);
            }
            while (count < size);

            dataStream.Position = 0;

            var formatter = new BinaryFormatter();
            obj = formatter.Deserialize(dataStream);
        }

        if (exceptionFlag[0] != 0)
        {
            var exception = (Exception)obj;
            throw exception;
        }

        return (T)obj;
    }
}