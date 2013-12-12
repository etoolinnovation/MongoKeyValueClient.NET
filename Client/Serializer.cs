using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class Serializer
    {
        private static readonly bool CompresionEnabled = Config.Instance.CompresionEnabled;            
        private static readonly bool JsonSerilization = Config.Instance.SerializationMode.ToUpper() == "JSON";


        internal static byte[] ToByteArray(Object obj)
        {
            if (obj == null) return null;

            byte[] data;

            if (JsonSerilization)
            {
                string jsonData = JsonConvert.SerializeObject(obj);
                data = GetBytes(jsonData);
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    var b = new BinaryFormatter();
                    b.Serialize(ms, obj);
                    data = ms.ToArray();
                    ms.Close();
                }
            }
            return CompresionEnabled ? Compression.Compress(data) : data;
        }

        internal static string ToJsonStringSerialize(byte[] serializedObject, Type T)
        {
            if (serializedObject == null) return null;

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);
           
            if (JsonSerilization)
            {
                return JsonConvert.DeserializeObject(GetString(serializedObject), T).ToString();
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(serializedObject, 0, serializedObject.Length);
                    ms.Seek(0, 0);
                    var b = new BinaryFormatter();
                    Object obj = b.Deserialize(ms);
                    ms.Close();
                    return JsonConvert.SerializeObject(obj);
                }
            }
            
        }

        internal static object ToObjectSerialize(byte[] serializedObject, Type T)
        {
            if (serializedObject == null) return null;

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            Object obj;

            if (JsonSerilization)
            {
                obj = JsonConvert.DeserializeObject(GetString(serializedObject), T);
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(serializedObject, 0, serializedObject.Length);
                    ms.Seek(0, 0);
                    var b = new BinaryFormatter();
                    obj = b.Deserialize(ms);
                    ms.Close();
                }
            }
            return Convert.ChangeType(obj, T);
        }

        internal static T ToObjectSerialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null) return default(T);

            if (CompresionEnabled) serializedObject = Compression.Decompress(serializedObject);

            Object obj;

            if (JsonSerilization)
            {
                obj = JsonConvert.DeserializeObject(GetString(serializedObject),typeof(T));
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    ms.Write(serializedObject, 0, serializedObject.Length);
                    ms.Seek(0, 0);
                    var b = new BinaryFormatter();
                    obj = b.Deserialize(ms);
                    ms.Close();
                }
            }
            return (T) obj;
        }

        static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}