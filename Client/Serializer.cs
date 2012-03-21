using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class Serializer
    {

        private static readonly bool CompresionEnabled = ConfigurationManager.AppSettings["MongoCacheClient_CompressionEnabled"] == "1";

        internal static byte[] ToByteArray(Object obj)
        {
            if (obj == null) return null;

            byte[] data;
            using (var ms = new MemoryStream())
            {
                var b = new BinaryFormatter();
                b.Serialize(ms, obj);
                data = ms.ToArray();
                ms.Close();
            }
            return CompresionEnabled ? Compression.Compress(data) : data;
        }

        internal static T ToObjectSerialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null) return default(T);

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            Object obj;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedObject, 0, serializedObject.Length);
                ms.Seek(0, 0);
                var b = new BinaryFormatter();
                obj = b.Deserialize(ms);
                ms.Close();
            }
            return (T)obj;
        }
    }
}
