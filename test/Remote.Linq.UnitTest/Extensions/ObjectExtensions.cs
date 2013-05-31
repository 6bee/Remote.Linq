using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Remote.Linq.UnitTest.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T CloneWithDataContractSerializer<T>(this T source)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(stream);
            }
        }

        public static T CloneWithNetDataContractSerializer<T>(this T source)
        {
            var serializer = new NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
