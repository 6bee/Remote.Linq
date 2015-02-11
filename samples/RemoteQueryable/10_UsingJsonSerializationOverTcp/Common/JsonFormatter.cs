// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Newtonsoft.Json;
    using Remote.Linq.TypeSystem;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class JsonFormatter
    {
        private readonly static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public static void Write(this Stream stream, object obj)
        {
            var typeInfo = new TypeInfo(obj.GetType());

            WriteInternal(stream, typeInfo);

            WriteInternal(stream, obj);
        }

        private static void WriteInternal(this Stream stream, object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSerializerSettings);

                byte[] data;
                using (var dataStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(dataStream, json);

                    dataStream.Position = 0;
                    data = dataStream.ToArray();
                }

                var size = data.LongLength;
                var sizeData = BitConverter.GetBytes(size);

                stream.Write(sizeData, 0, sizeData.Length);
                stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception: {0}", ex);
                throw;
            }
        }

        public static T Read<T>(this Stream stream)
        {
            var typeInfo = ReadInternal<TypeInfo>(stream);
            var type = typeInfo.Type;

            var obj = ReadInternal<T>(stream, type);
            return obj;
        }

        public static T ReadInternal<T>(this Stream stream, Type type = null)
        {
            try
            {
                var bytes = new byte[256];

                stream.Read(bytes, 0, 8);
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

                        int i = stream.Read(bytes, 0, length);
                        count += i;

                        dataStream.Write(bytes, 0, i);
                    } while (count < size);

                    dataStream.Position = 0;

                    var formatter = new BinaryFormatter();
                    var json = (string)formatter.Deserialize(dataStream);

                    obj = JsonConvert.DeserializeObject(json, type ?? typeof(T), _jsonSerializerSettings);
                }

                if (isException)
                {
                    var exception = (Exception)obj;
                    throw exception;
                }

                return (T)obj;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception: {0}", ex);
                throw;
            }
        }
    }
}
