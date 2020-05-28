// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.TypeSystem;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Remote.Linq;
    using System;
    using System.Collections;
    using System.IO;
    using System.Threading.Tasks;

    public static class BsonFormatter
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq();

        public static async Task WriteAsync(this Stream stream, object obj)
        {
            var typeInfo = new TypeInfo(obj.GetType());

            await WriteInternalAsync(stream, typeInfo).ConfigureAwait(false);

            await WriteInternalAsync(stream, obj).ConfigureAwait(false);
        }

        private static async Task WriteInternalAsync(this Stream stream, object obj)
        {
            byte[] data;
            using (var dataStream = new MemoryStream())
            using (var bsonWriter = new BsonDataWriter(dataStream))
            {
                var serializer = JsonSerializer.Create(_jsonSerializerSettings);
                serializer.Serialize(bsonWriter, obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            long size = data.LongLength;
            byte[] sizeData = BitConverter.GetBytes(size);

            byte datatype;
            if (obj is Exception)
            {
                datatype = 2;
            }
            else if (obj is IEnumerable && !(obj is string))
            {
                // https://github.com/JamesNK/Newtonsoft.Json/issues/1004
                // https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_Bson_BsonReader_ReadRootValueAsArray.htm
                datatype = 1;
            }
            else
            {
                datatype = 0;
            }

            stream.WriteByte(datatype);
            await stream.WriteAsync(sizeData, 0, sizeData.Length).ConfigureAwait(false);
            await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        public static async Task<T> ReadAsync<T>(this Stream stream)
        {
            var typeInfo = await ReadInternalAsync<TypeInfo>(stream).ConfigureAwait(false);
            var type = typeInfo.Type;

            T obj = await ReadInternalAsync<T>(stream, type).ConfigureAwait(false);
            return obj;
        }

        public static async Task<T> ReadInternalAsync<T>(this Stream stream, Type type = null)
        {
            int datatye = stream.ReadByte();
            if (datatye < 0)
            {
                throw new OperationCanceledException("network stream was closed by other party.");
            }

            bool isException = datatye == 2;

            // https://github.com/JamesNK/Newtonsoft.Json/issues/1004
            // https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_Bson_BsonReader_ReadRootValueAsArray.htm
            var isCollction = datatye == 1;

            byte[] bytes = new byte[256];

            await stream.ReadAsync(bytes, 0, 8).ConfigureAwait(false);
            long size = BitConverter.ToInt64(bytes, 0);

            object obj;
            using (var dataStream = new MemoryStream())
            {
                int count = 0;
                do
                {
                    int length = size - count < bytes.Length
                        ? (int)(size - count)
                        : bytes.Length;

                    int i = await stream.ReadAsync(bytes, 0, length).ConfigureAwait(false);
                    count += i;

                    dataStream.Write(bytes, 0, i);
                }
                while (count < size);

                dataStream.Position = 0;

                using var bsonReader = new BsonDataReader(dataStream) { ReadRootValueAsArray = isCollction };
                JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);
                obj = serializer.Deserialize(bsonReader, type ?? typeof(T));
            }

            if (isException)
            {
                var exception = (Exception)obj;
                throw exception;
            }

            return (T)obj;
        }
    }
}
