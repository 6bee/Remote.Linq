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
            TypeInfo typeInfo = new TypeInfo(obj.GetType(), false, false);

            await WriteInternalAsync(stream, typeInfo);

            await WriteInternalAsync(stream, obj);
        }

        private static async Task WriteInternalAsync(this Stream stream, object obj)
        {
            byte[] data;
            using (MemoryStream dataStream = new MemoryStream())
            {
                Type type = obj is Exception ? typeof(string) : obj.GetType();

                XmlSerializer xmlSerializer = new XmlSerializer(type);
                xmlSerializer.Serialize(dataStream, obj is Exception ? ((Exception)obj).Message : obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            long size = data.LongLength;
            byte[] sizeData = BitConverter.GetBytes(size);

            await stream.WriteAsync(sizeData, 0, sizeData.Length);
            stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public static async Task<T> ReadAsync<T>(this Stream stream)
        {
            TypeInfo typeInfo = await ReadInternalAsync<TypeInfo>(stream);
            Type type = typeInfo.Type;

            T obj = await ReadInternalAsync<T>(stream, type);
            return obj;
        }

        public static async Task<T> ReadInternalAsync<T>(this Stream stream, Type type = null)
        {
            byte[] bytes = new byte[256];

            await stream.ReadAsync(bytes, 0, 8);
            long size = BitConverter.ToInt64(bytes, 0);

            bool isException = stream.ReadByte() != 0;

            object obj;
            using (MemoryStream dataStream = new MemoryStream())
            {
                int count = 0;
                do
                {
                    int length = size - count < bytes.Length
                        ? (int)(size - count)
                        : bytes.Length;

                    int i = await stream.ReadAsync(bytes, 0, length);
                    count += i;

                    dataStream.Write(bytes, 0, i);
                }
                while (count < size);

                dataStream.Position = 0;

                Type serializedType = type ?? typeof(T);
                if (typeof(Exception).IsAssignableFrom(serializedType))
                {
                    serializedType = typeof(string);
                }

                XmlSerializer xmlSerializer = new XmlSerializer(serializedType);
                obj = xmlSerializer.Deserialize(dataStream);
            }

            if (isException)
            {
                string exceptionMessage = (string)obj;
                throw new Exception($"{type ?? typeof(T)}: '{exceptionMessage}'");
            }

            return (T)obj;
        }
    }
}
