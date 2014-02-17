using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.Mongo.KeyValueClient
{
    public class Client
    {
        private static readonly string ConnectionString = Config.Instance.ConnStr;

        private static string _dataBaseName = Config.Instance.Database;

        private static string _collectionName = ConfigurationManager.AppSettings["CompanyKey"] + Config.Instance.Collection;

        private static MongoCollection _col;
        private static MongoCollection _primaryCol;
        private static string _primaryConnectionString ;

        private static bool? _isReplicaSet = null;

        public Client(string preFix = "")
        {           
            if (!String.IsNullOrEmpty(preFix))
            {
                 _dataBaseName = Config.Instance.Database;
                 _collectionName = preFix + Config.Instance.Collection;
                _col = null;
                _primaryCol = null;
                _isReplicaSet = null;
            }
           
        }

        public MongoDatabase GetDb(string connectionString = null)
        {
            return GetServer(connectionString).GetDatabase(_dataBaseName);
        }

        private MongoServer GetServer(string connectionString = null)
        {
            var client = new MongoClient(connectionString ?? ConnectionString);
            return client.GetServer();
        }

        private MongoCollection Collection
        {
            get { return _col ?? (_col = GetDb().GetCollection(_collectionName)); }
        }

        private MongoCollection PrimaryCollection
        {
            get
            {
                if (_isReplicaSet == null)
                {
                    var server = GetServer();
                    _isReplicaSet = !String.IsNullOrEmpty(server.ReplicaSetName);
                }

                if (_isReplicaSet == false)
                {
                    return _col ?? (_col = GetDb().GetCollection(_collectionName));
                }
                else
                {
                    if (String.IsNullOrEmpty(_primaryConnectionString))
                    {
                        _primaryConnectionString = ConnectionString + ";readPreference=primary";
                    }
                    return _primaryCol ?? (_primaryCol = GetDb(_primaryConnectionString).GetCollection(_collectionName));
                }

            }
        }

        public bool Ping()
        {
            try
            {
                MongoServer s = GetServer();
                s.Connect();
                s.Ping();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        private static Type GetObjectType(CacheData cacheData)
        {
            string fullTypeName = cacheData.NameSpace == "System" ? cacheData.Type : String.Format("{0},{1}", cacheData.Type, cacheData.NameSpace);

            Type cacheType = Type.GetType(fullTypeName);

            if (cacheData.IsList)
            {
                Type listGenericType = typeof(List<>);
                cacheType = listGenericType.MakeGenericType(cacheType);
            }
            return cacheType;
        }

        #region GET
    
        public object Get(string key, Type T)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return Serializer.ToObjectSerialize(cacheItems.First().Data, T);
        }


        public object Get(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            var cacheData = cacheItems.First();

            return Serializer.ToObjectSerialize(cacheData.Data, GetObjectType(cacheData));
        }

        public object Get<T>(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            var cacheData = cacheItems.First();

            return Serializer.ToObjectSerialize<T>(cacheData.Data);
        }

        public object GetForWrite(string key, Type T)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return Serializer.ToObjectSerialize(cacheItems.First().Data, T);
        }

        public object GetForWrite(string key)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            var cacheData = cacheItems.First();

            return Serializer.ToObjectSerialize(cacheItems.First().Data, GetObjectType(cacheData));

        }

        public T GetForWrite<T>(string key)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return default(T);

            return Serializer.ToObjectSerialize<T>(cacheItems.First().Data);
        }

        public string GetAsString(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return (string)Serializer.ToJsonStringSerialize(cacheItems.First().Data, typeof(object));
        }

        #endregion

        #region GET LIST

        public IDictionary<string, object> Get(List<string> keyList)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.In("_id", new BsonArray(keyList));

            IDictionary<string, object> result = collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                                             item => Serializer.ToObjectSerialize(item.Data,GetObjectType(item)));
            foreach (string key in keyList.Where(key => !result.ContainsKey(key)))
            {
                result.Add(key, null);
            }

            return result;
        }
    
        public IDictionary<string, T> Get<T>(List<string> keyList)
        {
            MongoCollection collection = Collection;

            IMongoQuery query = Query.In("_id", new BsonArray(keyList));


            IDictionary<string, T> result = collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                                             item => Serializer.ToObjectSerialize<T>(item.Data));
            foreach (string key in keyList.Where(key => !result.ContainsKey(key)))
            {
                result.Add(key, default(T));
            }

            return result;
        }

        public IDictionary<string, object> Get(Dictionary<Type, List<string>> keys)
        {
            MongoCollection collection = Collection;

            var keyList = new List<string>();
            foreach (KeyValuePair<Type, List<string>> pair in keys)
            {
                keyList.AddRange(pair.Value);
            }

            IMongoQuery query = Query.In("_id", new BsonArray(keyList));
            var cacheDataCollection = collection.FindAs<CacheData>(query);

            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var cacheData in cacheDataCollection)
            {
                CacheData data = cacheData;
                var t = from k in keys where k.Value.Contains(data._id) select k;
                var keyValuePairs = t as IList<KeyValuePair<Type, List<string>>> ?? t.ToList();
                if (keyValuePairs.Any())
                {
                    Type T = keyValuePairs.First().Key;
                    result.Add(data._id, Serializer.ToObjectSerialize(data.Data, T));
                }
            }

            foreach (string key in keyList.Where(key => !result.ContainsKey(key)))
            {
                result.Add(key, null);
            }

            return result;
        }

        public IDictionary<string, T> GetRegex<T>(string pattern)
        {
            MongoCollection collection = Collection;

            IMongoQuery query = Query.Matches("_id", new BsonRegularExpression(pattern));

            return collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                    item =>
                                                                    Serializer.ToObjectSerialize<T>(item.Data));
        }

        public IDictionary<string, object> GetRegex(string pattern)
        {
            MongoCollection collection = Collection;

            IMongoQuery query = Query.Matches("_id", new BsonRegularExpression(pattern));

            return collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                    item =>
                                                                    Serializer.ToObjectSerialize(item.Data, GetObjectType(item)));
        }


      

        #endregion

        #region Add Remove RemoveAll

        public bool Add(string key, object data)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            string TypeName = string.Empty;
            string NameSpace = string.Empty;
            bool IsList = false;

            var list = data as IList;
            if (list != null)
            {
                var dataType = data.GetType();
                var itemType = dataType.GetGenericArguments()[0];
                TypeName = itemType.FullName;
                NameSpace = itemType.Namespace;
                IsList = true;
            }
            else
            {
                var dataType = data.GetType();
                TypeName = dataType.FullName;
                NameSpace = dataType.Namespace;
                IsList = false;
            }

            var result = collection.FindAndModify(query, null, Update.Set("Data", Serializer.ObjectToString(data)).Set("Type",TypeName).Set("IsList", IsList).Set("NameSpace", NameSpace), false, true);
            
            if (!String.IsNullOrEmpty(result.ErrorMessage))
                throw new Exception(result.ErrorMessage);
            
            return true;
        }

        public bool Remove(string key)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);
            var result = collection.Remove(query);
            
            if (!String.IsNullOrEmpty(result.ErrorMessage))
                throw new Exception(result.ErrorMessage);
            
            return true;
        }

        public void RemoveAll()
        {
            MongoCollection collection = PrimaryCollection;
            collection.RemoveAll();
        }

        public List<string> GetAllKeys()
        {
            MongoCollection collection = Collection;
            return collection.FindAllAs<CacheData>().SetFields("_id").ToList().Select(data => data._id).ToList();
        }

        #endregion

        public MongoCursor<CacheData> GetAllKeysAsCursor()
        {
            MongoCollection collection = Collection;
            return collection.FindAllAs<CacheData>().SetFields("_id");
        }

        public MongoCursor<CacheData> GetKeysRegex(string pattern)
        {
            MongoCollection collection = Collection;

            IMongoQuery query = Query.Matches("_id", new BsonRegularExpression(pattern));
            return collection.FindAs<CacheData>(query).SetFields("_id");
        }

        public Dictionary<string,long> GetAllKeysWithSize()
        {
            MongoCollection collection = Collection;
            var result = new Dictionary<string, long>();

            foreach (CacheData cacheData in collection.FindAllAs<CacheData>())
            {
                result.Add(cacheData._id, cacheData.Data.Length / 1024);
            }

            return result;
        }

     
        public long SizeAsKb(string key)
        {
            return DecompressSizeAsKb(key);
        }


        public long DecompressSizeAsKb(string key)
        {
                        
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return 0;

            if (!Config.Instance.CompresionEnabled)
                return SizeAsKb(cacheItems.First().Data);
           
            return StringCompressor.DecompressString(cacheItems.First().Data).Length / 1024;
        }

        #region Nested type: CacheData

        [Serializable]
        public class CacheData
        {
            [BsonId]
            public string _id { get; set; }

            public string Data { get; set; }

            public string Type { get; set; }

            public string NameSpace { get; set; }

            public bool IsList { get; set; }
        }

        #endregion
    }
}