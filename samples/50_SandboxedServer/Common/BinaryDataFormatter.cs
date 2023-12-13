// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;

    public static class BinaryDataFormatter
    {
        [SecuritySafeCritical]
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

        [SecuritySafeCritical]
        public static T Read<T>(this Stream stream)
        {
            byte[] bytes = new byte[256];

            int i = stream.Read(bytes, 0, 8);
            if (i != 8)
            {
                throw new IOException("Failed to read message size from stream.");
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

                    i = stream.Read(bytes, 0, length);
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
    }
}