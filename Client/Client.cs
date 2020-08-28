using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace EtoolTech.Mongo.KeyValueClient
{
    public class Client
    {
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"];
        private static string _dataBaseName = ConfigurationManager.AppSettings["MongoKeyValueClient_Database"];
        private static string _collectionName = ConfigurationManager.AppSettings["CompanyKey"] + ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"];

        private static IMongoCollection<CacheData> _col;
        private static IMongoCollection<CacheData> _primaryCol;
        private static MongoClientSettings _primaryConnectionSettings;

        private static bool? _isReplicaSet;

        private static readonly bool SerializeProtoBuf = ConfigurationManager.AppSettings["MongoKeyValueClient_SerializeProtoBuf"] == "1";
        private static readonly bool CompresionEnabled = ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] == "1";

        private ISerializer _serializer;

        public Client(string preFix = "")
        {
            if (!String.IsNullOrEmpty(preFix))
            {
                _dataBaseName = ConfigurationManager.AppSettings["MongoKeyValueClient_Database"];
                _collectionName = preFix + ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"];
                _col = null;
                _primaryCol = null;
                _isReplicaSet = null;

            }
            _serializer = SerializeProtoBuf ? (ISerializer)new SerializerProtoBuf() : new SerializerBinary();
        }

        public void ChangeSerializer()
        {
            _serializer = SerializeProtoBuf ? (ISerializer)new SerializerBinary() : new SerializerProtoBuf();
        }

        private IMongoDatabase GetDb(string connectionString = null)
        {
            return GetServer(connectionString, false).GetDatabase(_dataBaseName);
        }

        private IMongoDatabase GetDb(MongoClientSettings settings)
        {
            return GetServer(settings).GetDatabase(_dataBaseName);
        }

        private MongoClient GetServer(string connectionString = null, bool fromPrimary = false)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(MongoUrl.Create(connectionString ?? ConnectionString));
            return GetServer(settings, fromPrimary);
        }

        private MongoClient GetServer(MongoClientSettings settings, bool fromPrimary = false)
        {
            if (fromPrimary)
                settings.ReadPreference = ReadPreference.Primary;

            var client = new MongoClient(settings);
            return client;
        }

        private IMongoCollection<CacheData> Collection
        {
            get { return _col ?? (_col = GetDb().GetCollection<CacheData>(_collectionName)); }
        }

        private IMongoCollection<CacheData> PrimaryCollection
        {
            get
            {
                if (_isReplicaSet == null)
                {
                    var server = GetServer();
                    _isReplicaSet = !String.IsNullOrEmpty(server.Settings.ReplicaSetName);
                }

                if (_isReplicaSet == false)
                {
                    return _col ?? (_col = GetDb().GetCollection<CacheData>(_collectionName));
                }

                if (_primaryConnectionSettings == null)
                {
                    var builder = new MongoUrlBuilder(ConnectionString);
                    MongoUrl url = builder.ToMongoUrl();
                    _primaryConnectionSettings = MongoClientSettings.FromUrl(url);
                    _primaryConnectionSettings.ReadPreference = ReadPreference.Primary;
                }

                return _primaryCol ?? (_primaryCol = GetDb(_primaryConnectionSettings).GetCollection<CacheData>(_collectionName));
            }
        }

        public bool Ping()
        {
            try
            {
                var s = GetServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public T Get<T>(string key)
        {
            var collection = Collection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return default(T);

            return (T) _serializer.ToObjectSerialize(typeof (T), cacheItems.First().Data);
        }

        public object Get(string key)
        {
            var collection = Collection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return default(object);

            Type type = (Type)Deserialize(cacheItems.First().Type);
            return _serializer.ToObjectSerialize(type, cacheItems.First().Data);
        }

        public T GetForWrite<T>(string key)
        {
            var collection = PrimaryCollection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.WithReadPreference(ReadPreference.Primary).Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return default(T);

            return (T) _serializer.ToObjectSerialize(typeof (T), cacheItems.First().Data);
        }

        public object GetForWrite(string key)
        {
            var collection = Collection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.WithReadPreference(ReadPreference.Primary).Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return default(object);

            Type type = (Type)Deserialize(cacheItems.First().Type);
            return _serializer.ToObjectSerialize(type, cacheItems.First().Data);
        }

        public IDictionary<string, object> GetForWrite(List<string> keyList)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            var collection = PrimaryCollection;
            var query = Builders<CacheData>.Filter.In("_id", keyList);

            var data = collection.WithReadPreference(ReadPreference.Primary).Find(query).ToListAsync().Result;

            foreach (var cacheData in data)
            {
                Type type = (Type)Deserialize(cacheData.Type);
                result.Add(cacheData._id, _serializer.ToObjectSerialize(type ?? typeof(object), cacheData.Data));
            }

            foreach (string key in keyList.Where(Key => !result.ContainsKey(Key)))
            {
                result.Add(key, default(object));
            }

            return result;
        }

        public bool Add(string key, object data, Type type)
        {
            var collection = PrimaryCollection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            collection.FindOneAndUpdateAsync(
                query, Builders<CacheData>.Update.Set(c => c.Data, _serializer.ToByteArray(data)).Set(c => c.Type, Serialize(type)),
                new FindOneAndUpdateOptions<CacheData> {IsUpsert = true, ReturnDocument = ReturnDocument.After}).GetAwaiter().GetResult();

            return true;
        }

        public bool Remove(string key)
        {
            var collection = PrimaryCollection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);
            collection.FindOneAndDeleteAsync(query, new FindOneAndDeleteOptions<CacheData>()).GetAwaiter().GetResult();
            return true;
        }

        public void RemoveAll()
        {
            var collection = PrimaryCollection;
            collection.DeleteManyAsync(new BsonDocument());
        }

        public List<string> GetAllKeys()
        {
            var collection = Collection;
            return collection.Find(new BsonDocument()).Project(c => c._id).ToListAsync().Result;
        }

        public IEnumerable<CacheData> GetAllKeysAsCursor()
        {
            var collection = Collection;
            return collection.Find(new BsonDocument()).Project<CacheData>(Builders<CacheData>.Projection.Exclude(c => c.Data)).ToListAsync().Result;
        }

        public IEnumerable<CacheData> GetKeysRegex(string pattern)
        {
            var collection = Collection;
            return collection.Find(Builders<CacheData>.Filter.Regex("_id", new BsonRegularExpression(pattern))).Project<CacheData>(Builders<CacheData>
                             .Projection.Exclude(c => c.Data)).ToListAsync().Result;
        }

        public Dictionary<string, long> GetAllKeysWithSize()
        {
            var collection = Collection;
            var result = new Dictionary<string, long>();

            foreach (CacheData cacheData in collection.Find(new BsonDocument()).ToListAsync().Result)
            {
                result.Add(cacheData._id, cacheData.Data.Length/1024);
            }

            return result;
        }

        public IDictionary<string, object> Get(List<string> keyList)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            
            var query = Builders<CacheData>.Filter.In("_id", keyList);
            var data = Collection.Find(query).ToListAsync().Result;

            foreach (var cacheData in data)
            {
                Type type = (Type)Deserialize(cacheData.Type);
                result.Add(cacheData._id, _serializer.ToObjectSerialize(type ?? typeof(object), cacheData.Data));
            }

            foreach (string key in keyList.Where(Key => !result.ContainsKey(Key)))
            {
                result.Add(key, default(object));
            }

            return result;
        }

        public IDictionary<string, object> Get(IDictionary<Type, List<string>> keyListByType)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (var keyList in keyListByType)
            {
                var query = Builders<CacheData>.Filter.In("_id", keyList.Value);

                var data = Collection.Find(query).ToListAsync().Result;

                foreach (var cacheData in data)
                {
                    result.Add(cacheData._id, _serializer.ToObjectSerialize(keyList.Key, cacheData.Data));
                }

                foreach (string key in keyList.Value.Where(Key => !result.ContainsKey(Key)))
                {
                    result.Add(key, default(object));
                }
            }

            return result;
        }

        public IDictionary<string, object> GetRegex(string pattern)
        {
            var query = Builders<CacheData>.Filter.Regex("_id", new BsonRegularExpression(pattern));

            var data = Collection.Find(query).ToListAsync().Result;

            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var cacheData in data)
            {
                result.Add(cacheData._id, _serializer.ToObjectSerialize(typeof (object), cacheData.Data));
            }

            return result;
        }

        public long SizeAsKb(string key)
        {
            var collection = Collection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return 0;

            return cacheItems.First().Data.LongLength/1024;
        }

        public long DecompressSizeAsKb(string key)
        {
            if (ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] != "1")
                return SizeAsKb(key);

            var collection = Collection;
            var query = Builders<CacheData>.Filter.Eq("_id", key);

            List<CacheData> cacheItems = collection.Find(query).ToListAsync().Result;

            if (!cacheItems.Any())
                return 0;

            return Compression.Decompress(cacheItems.First().Data).LongLength/1024;
        }

        public byte[] Serialize(object obj)
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

        public object Deserialize(byte[] serializedObject)
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

        #region Nested type: CacheData

        [Serializable]
        public class CacheData
        {
            [BsonId]
            public string _id { get; set; }

            public byte[] Data { get; set; }

            public byte[] Type { get; set; }
        }

        #endregion
    }
}