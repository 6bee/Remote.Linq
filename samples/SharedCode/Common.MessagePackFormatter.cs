// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using MessagePack;
    using MessagePack.Resolvers;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public static class MessagePackFormatter
    {
        private static readonly MessagePackSerializerOptions _options =
            MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Envelope
        {
            public object Body { get; set; }
        }

        /// <summary>
        /// Write serialized object to stream using MessagePack typeless object serializer.
        /// </summary>
        public static async ValueTask WriteAsync(this Stream stream, object obj, CancellationToken cancellation = default)
        {
            var bin = MessagePackSerializer.Serialize(new Envelope { Body = obj }, _options);
            var len = BitConverter.GetBytes(bin.Length);
            stream.WriteByte((byte)len.Length);
            await stream.WriteAsync(len, cancellation).ConfigureAwait(false);
            await stream.WriteAsync(bin, cancellation).ConfigureAwait(false);
        }

        /// <summary>
        /// Read and deserialze object from stream using MessagePack typeless object serializer.
        /// </summary>
        public static async ValueTask<T> ReadAsync<T>(this Stream stream, CancellationToken cancellation = default)
        {
            var c = stream.ReadByte();
            var len = new byte[c];
            if (await stream.ReadAsync(len, 0, len.Length, cancellation).ConfigureAwait(false) != len.Length)
            {
                throw new IOException("Failed to read data");
            }

            var messageSize = BitConverter.ToInt32(len);
            if (messageSize == 0)
            {
                throw new IOException("Unexpected empty message");
            }

            var bin = new byte[messageSize];
            var buffer = new byte[512];
            var count = 0;
            do
            {
                var length = Math.Min(buffer.Length, messageSize - count);
                var size = await stream.ReadAsync(buffer, 0, length, cancellation).ConfigureAwait(false);
                Array.Copy(buffer, 0, bin, count, size);
                count += size;
            }
            while (count < messageSize);

            var envelope = MessagePackSerializer.Deserialize<Envelope>(bin, _options);
            return envelope.Body is Exception ex
                ? throw ex
                : (T)envelope.Body;
        }
    }
}