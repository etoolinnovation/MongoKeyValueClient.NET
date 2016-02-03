using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class SerializerBinary : ISerializer
    {
        private static readonly bool CompresionEnabled = ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] == "1";

        public byte[] ToByteArray(Object obj)
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

        public object ToObjectSerialize(Type type, byte[] serializedObject)
        {
            if (serializedObject == null) return default(object);

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            object obj;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedObject, 0, serializedObject.Length);
                ms.Seek(0, 0);
                var b = new BinaryFormatter();
                obj = b.Deserialize(ms);
                ms.Close();
            }
            return obj;
        }
    }
}