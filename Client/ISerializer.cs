using System;

namespace EtoolTech.Mongo.KeyValueClient
{
    public interface ISerializer
    {
        byte[] ToByteArray(object obj);
        object ToObjectSerialize(Type type, byte[] serializedObject);
    }
}
