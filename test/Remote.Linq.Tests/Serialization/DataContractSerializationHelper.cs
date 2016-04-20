// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using System.IO;
    using System.Runtime.Serialization;

    public static class DataContractSerializationHelper
    {
        public static T Serialize<T>(this T graph)
        {
            var serializer = new DataContractSerializer(typeof(T));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
