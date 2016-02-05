using System;
using System.Configuration;
using System.IO;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class SerializerProtoBuf : ISerializer
    {
        private static readonly bool CompresionEnabled = ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] == "1";

        public byte[] ToByteArray(object obj)
        {
            MemoryStream rawOutput = new MemoryStream();
            ProtoBuf.Serializer.NonGeneric.Serialize(rawOutput, obj);
            byte[] data = rawOutput.ToArray();

            return CompresionEnabled ? Compression.Compress(data) : data;
        }

        public object ToObjectSerialize(Type type, byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(object);

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            MemoryStream rawOutput = new MemoryStream(serializedObject);

            return ProtoBuf.Serializer.NonGeneric.Deserialize(type, rawOutput);
        }
    }
}
