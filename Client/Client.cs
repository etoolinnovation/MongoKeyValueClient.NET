using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        
        private static Dictionary<string,Type> _objecTypesCache = new Dictionary<string, Type>(); 
        private static Type GetObjectType(CacheData cacheData)
        {

            bool isNestedType = false;
            string nestedTypeName = string.Empty;

            if (cacheData.Type.Contains("+"))
            {
                string[] tmpArray = cacheData.Type.Split('+');
                cacheData.Type = tmpArray[0];
                nestedTypeName = tmpArray[1];
                isNestedType = true;
            }
            
            var fullTypeName = cacheData.Assembly == "System" ? cacheData.Type : String.Format("{0},{1}", cacheData.Type, cacheData.Assembly);


            Type cacheType = Type.GetType(fullTypeName);

            if (isNestedType)
            {
                cacheType = cacheType.GetNestedType(nestedTypeName);
            }

            
            if (cacheData.DataType == "OBJECT") return cacheType;


            var listGenericType = typeof(List<>);
            return listGenericType.MakeGenericType(cacheType);            
        }

        #region GET
    
        public object Get(string key, Type T)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return Serializer.ToObjectDeserialize(cacheItems.First().Data, T);
        }


        public object Get(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            var cacheData = cacheItems.First();

            return Serializer.ToObjectDeserialize(cacheData.Data, GetObjectType(cacheData));
        }

        public T Get<T>(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return default(T);

            var cacheData = cacheItems.First();

            return Serializer.ToObjectDeserialize<T>(cacheData.Data);
        }

        public object GetForWrite(string key, Type T)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return Serializer.ToObjectDeserialize(cacheItems.First().Data, T);
        }

        public object GetForWrite(string key)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            var cacheData = cacheItems.First();

            return Serializer.ToObjectDeserialize(cacheItems.First().Data, GetObjectType(cacheData));

        }

        public T GetForWrite<T>(string key)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return default(T);

            return Serializer.ToObjectDeserialize<T>(cacheItems.First().Data);
        }

        public string GetAsString(string key)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return (string)Serializer.ToJsonStringDeserialize(cacheItems.First().Data, typeof(object));
        }

        #endregion

        #region GET LIST

        public IDictionary<string, object> Get(List<string> keyList)
        {
            MongoCollection collection = Collection;
            IMongoQuery query = Query.In("_id", new BsonArray(keyList));

            IDictionary<string, object> result = collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                                             item => Serializer.ToObjectDeserialize(item.Data, GetObjectType(item)));
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
                                                                                             item => Serializer.ToObjectDeserialize<T>(item.Data));
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
                    result.Add(data._id, Serializer.ToObjectDeserialize(data.Data, T));
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
                                                                    Serializer.ToObjectDeserialize<T>(item.Data));
        }

        public IDictionary<string, object> GetRegex(string pattern)
        {
            MongoCollection collection = Collection;

            IMongoQuery query = Query.Matches("_id", new BsonRegularExpression(pattern));

            return collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                    item =>
                                                                    Serializer.ToObjectDeserialize(item.Data, GetObjectType(item)));
        }


      

        #endregion

        #region Add Remove RemoveAll

        public bool Add(string key, object data)
        {
            MongoCollection collection = PrimaryCollection;
            IMongoQuery query = Query.EQ("_id", key);

            string TypeName = string.Empty;
            string AssemblyName = string.Empty;
            string DataType = string.Empty;

            var list = data as IList;
            var dic = data as IDictionary;
            if (list != null)
            {
                var dataType = data.GetType();
                var itemType = dataType.GetGenericArguments()[0];
                TypeName = itemType.FullName;
                AssemblyName = itemType.Assembly.FullName.Split(',')[0].Trim();
                DataType = "LIST";
            }
            else if (dic != null)
            {
                DataType = "DICT";
                var dataType = data.GetType();
                var itemType = dataType.GetGenericArguments()[0];
                var list1 = itemType as IList;
                if (list1 != null)
                {
                    itemType = itemType.GetGenericArguments()[0];                    
                }

                var itemType2 = dataType.GetGenericArguments()[1];
                var list2 = itemType2 as IList;
                if (list2 != null)
                {
                    itemType2 = itemType.GetGenericArguments()[0];
                }

                TypeName = itemType.FullName + "," + itemType2.FullName;
                AssemblyName = itemType.Assembly.FullName.Split(',')[0].Trim() + "," + itemType2.Assembly.FullName.Split(',')[0].Trim();
               
            }
            else
            {
                var dataType = data.GetType();
                TypeName = dataType.FullName;
                AssemblyName = dataType.Namespace;
                DataType = "OBJECT";
            }

            var result = collection.FindAndModify(query, null, Update.Set("Data", Serializer.ObjectToString(data)).Set("Type",TypeName).Set("DataType", DataType).Set("Assembly", AssemblyName), false, true);
            
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

            public string Assembly { get; set; }

            public string DataType { get; set; }
        }

        #endregion
    }
}