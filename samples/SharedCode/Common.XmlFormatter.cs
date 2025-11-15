// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common;

using Aqua.TypeSystem;
using Remote.Linq;
using System.IO;
using System.Xml.Serialization;

public static class XmlFormatter
{
    public static async ValueTask WriteAsync(this Stream stream, object obj, CancellationToken cancellation = default)
    {
        var typeInfo = new TypeInfo(obj.GetType(), false, false);

        await WriteInternalAsync(stream, typeInfo, cancellation).ConfigureAwait(false);

        await WriteInternalAsync(stream, obj, cancellation).ConfigureAwait(false);
    }

    private static async ValueTask WriteInternalAsync(this Stream stream, object obj, CancellationToken cancellation)
    {
        byte[] data;
        using (var dataStream = new MemoryStream())
        {
            Type type = obj is Exception ? typeof(string) : obj.GetType();

            var xmlSerializer = new XmlSerializer(type);
            xmlSerializer.Serialize(dataStream, obj is Exception exception ? exception.Message : obj);
            dataStream.Position = 0;
            data = dataStream.ToArray();
        }

        long size = data.LongLength;
        byte[] sizeData = BitConverter.GetBytes(size);

        await stream.WriteAsync(sizeData, 0, sizeData.Length, cancellation).ConfigureAwait(false);
        stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
        await stream.WriteAsync(data, 0, data.Length, cancellation).ConfigureAwait(false);
    }

    public static async ValueTask<T> ReadAsync<T>(this Stream stream, CancellationToken cancellation = default)
    {
        var typeInfo = await ReadInternalAsync<TypeInfo>(stream, null, cancellation).ConfigureAwait(false);
        var type = typeInfo.ToType();

        T obj = await ReadInternalAsync<T>(stream, type, cancellation).ConfigureAwait(false);
        return obj;
    }

    private static async ValueTask<T> ReadInternalAsync<T>(this Stream stream, Type type, CancellationToken cancellation)
    {
        byte[] bytes = new byte[256];

        await stream.ReadAsync(bytes, 0, 8, cancellation).ConfigureAwait(false);
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

                int i = await stream.ReadAsync(bytes, 0, length, cancellation).ConfigureAwait(false);
                count += i;

                await dataStream.WriteAsync(bytes, 0, i, cancellation).ConfigureAwait(false);
            }
            while (count < size);

            dataStream.Position = 0;

            Type serializedType = type ?? typeof(T);
            if (typeof(Exception).IsAssignableFrom(serializedType))
            {
                serializedType = typeof(string);
            }

            var xmlSerializer = new XmlSerializer(serializedType);
            obj = xmlSerializer.Deserialize(dataStream);
        }

        if (isException)
        {
            string exceptionMessage = (string)obj;
            throw new RemoteLinqException($"{type ?? typeof(T)}: '{exceptionMessage}'");
        }

        return (T)obj;
    }
}