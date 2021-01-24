// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.TypeSystem;
    using Remote.Linq;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ProtobufNetFormatter
    {
        private static readonly ProtoBuf.Meta.RuntimeTypeModel _configuration = ProtoBufTypeModel.ConfigureRemoteLinq();

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
                _configuration.Serialize(dataStream, obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            long size = data.LongLength;
            byte[] sizeData = BitConverter.GetBytes(size);

            var datatype = obj is Exception ? (byte)1 : (byte)0;

            stream.WriteByte(datatype);
            await stream.WriteAsync(sizeData, 0, sizeData.Length, cancellation).ConfigureAwait(false);
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
            int datatye = stream.ReadByte();
            if (datatye < 0)
            {
                throw new OperationCanceledException("network stream was closed by other party.");
            }

            bool isException = datatye == 1;
            byte[] bytes = new byte[256];

            await stream.ReadAsync(bytes, 0, 8, cancellation).ConfigureAwait(false);
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

                        int i = await stream.ReadAsync(bytes, 0, length, cancellation).ConfigureAwait(false);
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
