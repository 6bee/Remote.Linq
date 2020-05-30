// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.TypeSystem;
    using Remote.Linq;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class ProtobufNetFormatter
    {
        private static readonly ProtoBuf.Meta.RuntimeTypeModel _configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

        public static async Task WriteAsync(this Stream stream, object obj)
        {
            var typeInfo = new TypeInfo(obj.GetType(), false, false);

            await WriteInternalAsync(stream, typeInfo).ConfigureAwait(false);

            await WriteInternalAsync(stream, obj).ConfigureAwait(false);
        }

        private static async Task WriteInternalAsync(this Stream stream, object obj)
        {
            byte[] data;
            using (var dataStream = new MemoryStream())
            {
                _configuration.Serialize(dataStream, obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            long size = data.LongLength;
            byte[] sizeData = BitConverter.GetBytes(size);

            var datatype = obj is Exception ? (byte)1 : (byte)0;

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

            bool isException = datatye == 1;
            byte[] bytes = new byte[256];

            await stream.ReadAsync(bytes, 0, 8).ConfigureAwait(false);
            long size = BitConverter.ToInt64(bytes, 0);

            object obj;
            if (size > 0)
            {
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
                    obj = _configuration.Deserialize(dataStream, null, type ?? typeof(T));
                }
            }
            else
            {
                obj = Activator.CreateInstance(type);
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
