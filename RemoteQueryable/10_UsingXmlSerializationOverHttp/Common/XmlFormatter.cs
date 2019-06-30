// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.TypeSystem;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    public static class XmlFormatter
    {
        public static async Task WriteAsync(this Stream stream, object obj)
        {
            var typeInfo = new TypeInfo(obj.GetType(), false, false);

            await WriteInternalAsync(stream, typeInfo);

            await WriteInternalAsync(stream, obj);
        }

        private static async Task WriteInternalAsync(this Stream stream, object obj)
        {
            byte[] data;
            using (var dataStream = new MemoryStream())
            {
                var type = obj is Exception ? typeof(string) : obj.GetType();

                var xmlSerializer = new XmlSerializer(type);
                xmlSerializer.Serialize(dataStream, obj is Exception ? ((Exception)obj).Message : obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            var size = data.LongLength;
            var sizeData = BitConverter.GetBytes(size);

            await stream.WriteAsync(sizeData, 0, sizeData.Length);
            stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public static async Task<T> ReadAsync<T>(this Stream stream)
        {
            var typeInfo = await ReadInternalAsync<TypeInfo>(stream);
            var type = typeInfo.Type;

            var obj = await ReadInternalAsync<T>(stream, type);
            return obj;
        }

        public static async Task<T> ReadInternalAsync<T>(this Stream stream, Type type = null)
        {
            var bytes = new byte[256];

            await stream.ReadAsync(bytes, 0, 8);
            var size = BitConverter.ToInt64(bytes, 0);

            var isException = stream.ReadByte() != 0;

            object obj;
            using (var dataStream = new MemoryStream())
            {
                int count = 0;
                do
                {
                    var length = size - count < bytes.Length
                        ? (int)(size - count)
                        : bytes.Length;

                    int i = await stream.ReadAsync(bytes, 0, length);
                    count += i;

                    dataStream.Write(bytes, 0, i);
                } while (count < size);

                dataStream.Position = 0;

                var serializedType = type ?? typeof(T);
                if (typeof(Exception).IsAssignableFrom(serializedType))
                {
                    serializedType = typeof(string);
                }

                var xmlSerializer = new XmlSerializer(serializedType);
                obj = xmlSerializer.Deserialize(dataStream);
            }

            if (isException)
            {
                var exceptionMessage = (string)obj;
                throw new Exception(string.Format("{0}: '{1}'", type ?? typeof(T), exceptionMessage));
            }

            return (T)obj;
        }
    }
}
