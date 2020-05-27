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

    public static class BsonFormatter
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }.ConfigureRemoteLinq();

        public static void Write(this Stream stream, object obj)
        {
            TypeInfo typeInfo = new TypeInfo(obj.GetType());

            WriteInternal(stream, typeInfo);

            WriteInternal(stream, obj);
        }

        private static void WriteInternal(this Stream stream, object obj)
        {
            byte[] data;
            using (var dataStream = new MemoryStream())
            using (var bsonWriter = new BsonDataWriter(dataStream))
            {
                JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);
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
            stream.Write(sizeData, 0, sizeData.Length);
            stream.Write(data, 0, data.Length);
        }

        public static T Read<T>(this Stream stream)
        {
            TypeInfo typeInfo = ReadInternal<TypeInfo>(stream);
            Type type = typeInfo.Type;

            T obj = ReadInternal<T>(stream, type);
            return obj;
        }

        public static T ReadInternal<T>(this Stream stream, Type type = null)
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

            stream.Read(bytes, 0, 8);
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

                    int i = stream.Read(bytes, 0, length);
                    count += i;

                    dataStream.Write(bytes, 0, i);
                }
                while (count < size);

                dataStream.Position = 0;

                using (var bsonReader = new BsonDataReader(dataStream) { ReadRootValueAsArray = isCollction })
                {
                    JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);
                    obj = serializer.Deserialize(bsonReader, type ?? typeof(T));
                }
            }

            if (isException)
            {
                Exception exception = (Exception)obj;
                throw exception;
            }

            return (T)obj;
        }
    }
}
