using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class Serializer
    {
        private static readonly bool CompresionEnabled = Config.Instance.CompresionEnabled;

        internal static string ObjectToString(Object obj)
        {
            string data = JsonConvert.SerializeObject(obj);
            return CompresionEnabled ? StringCompressor.CompressString(data) : data;            
        }

    

        internal static string ToJsonStringDeserialize(string serializedObject, Type T)
        {
            if (serializedObject == null) return null;

            if (CompresionEnabled) serializedObject = StringCompressor.DecompressString(serializedObject);

            return JsonConvert.DeserializeObject(serializedObject, T).ToString();

        }

    

        internal static object ToObjectDeserialize(string serializedObject, Type T)
        {
            if (serializedObject == null) return null;

            if (CompresionEnabled) serializedObject = StringCompressor.DecompressString(serializedObject);

            Object obj = JsonConvert.DeserializeObject(serializedObject, T);
            return Convert.ChangeType(obj, T);
           
        }

        internal static T ToObjectDeserialize<T>(string serializedObject)
        {
            return (T) ToObjectDeserialize(serializedObject, typeof(T));
        }

  

    }
}