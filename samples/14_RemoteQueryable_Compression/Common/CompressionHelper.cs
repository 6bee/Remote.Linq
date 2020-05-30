// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using Aqua.Dynamic;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.Serialization.Formatters.Binary;

    public class CompressionHelper
    {
        public IEnumerable<DynamicObject> Decompress(byte[] compressedData)
        {
            using var decompressedStream = new MemoryStream();
            using (var compressedStream = new MemoryStream())
            {
                compressedStream.Write(compressedData, 0, compressedData.Length);
                compressedStream.Seek(0, SeekOrigin.Begin);

                using var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                decompressionStream.CopyTo(decompressedStream);
            }

            decompressedStream.Seek(0, SeekOrigin.Begin);

            var formatter = new BinaryFormatter();
            object obj = formatter.Deserialize(decompressedStream);
            return (IEnumerable<DynamicObject>)obj;
        }

        public byte[] Compress(IEnumerable<DynamicObject> graph)
        {
            using var compressedStream = new MemoryStream();
            using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(compressionStream, graph);
            }

            byte[] compressedData = compressedStream.ToArray();
            return compressedData;
        }
    }
}
