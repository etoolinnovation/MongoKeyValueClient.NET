using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace OrangeTech.Orca.Tazzy.SharedCacheClient
{
    internal class Serializer
    {

        private static readonly bool CompresionEnabled = ConfigurationManager.AppSettings["MongoCacheClient_CompressionEnabled"] == "1";

        internal static byte[] ToByteArray(Object obj)
        {
            if (obj == null) return null;

            var ms = new MemoryStream();
            var b = new BinaryFormatter();
            b.Serialize(ms, obj);
            byte[] data = ms.ToArray();
            ms.Close();
            return CompresionEnabled ? Compression.Compress(data) : data;
        }

        internal static T ToObjectSerialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null) return default(T);

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            var ms = new MemoryStream();
            ms.Write(serializedObject, 0, serializedObject.Length);
            ms.Seek(0, 0);
            var b = new BinaryFormatter();
            Object obj = b.Deserialize(ms);
            ms.Close();            
            return (T)obj;
        }
    }
}
