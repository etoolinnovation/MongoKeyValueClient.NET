using System;
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
        private static readonly string ConnectionString =
            ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"];

        private static string _dataBaseName = ConfigurationManager.AppSettings["MongoKeyValueClient_Database"];

        private static string _collectionName = ConfigurationManager.AppSettings["CompanyKey"] +
                                                ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"];

        private static MongoCollection _col;

        public Client(string preFix = "")
        {           
			if (!String.IsNullOrEmpty(preFix))
            {
                 _dataBaseName = ConfigurationManager.AppSettings["MongoKeyValueClient_Database"];
                _collectionName = preFix + ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"];
                _col = null;
            }
           
        }


        public MongoDatabase Db
        {
            get { return Server.GetDatabase(_dataBaseName); }
        }

        private MongoServer Server
        {
            get { return MongoServer.Create(ConnectionString); }
        }

        private MongoCollection Collection
        {
            get { return _col ?? (_col = Db.GetCollection(_collectionName)); }
        }

        public bool Ping()
        {
            try
            {
                MongoServer s = Server;
                s.Connect();
                s.Ping();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object Get(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return null;

            return Serializer.ToObjectSerialize<object>(cacheItems.First().Data);
        }


        public bool Add(string key, object data)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);
            var result = collection.FindAndModify(query, null, Update.Set("Data", Serializer.ToByteArray(data)), false, true);
			
			if (!String.IsNullOrEmpty(result.ErrorMessage))
				throw new Exception(result.ErrorMessage);
			
            return true;
        }

        public bool Remove(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);
            var result = collection.Remove(query, SafeMode.True);
			
			if (!String.IsNullOrEmpty(result.ErrorMessage))
				throw new Exception(result.ErrorMessage);
			
            return true;
        }

        public List<string> GetAllKeys()
        {
            MongoCollection collection = Collection;
            return collection.FindAllAs<CacheData>().SetFields("_id").ToList().Select(data => data._id).ToList();
        }

        public Dictionary<string,long> GetAllKeysWithSize()
        {
            MongoCollection collection = Collection;
            var result = new Dictionary<string, long>();

            foreach (CacheData cacheData in collection.FindAllAs<CacheData>())
            {
                result.Add(cacheData._id, cacheData.Data.LongLength / 1024);
            }

            return result;
        }

        public IDictionary<string, object> Get(List<string> keyList)
        {
            MongoCollection collection = Collection;

            QueryConditionList query = Query.In("_id", new BsonArray(keyList));
			

            IDictionary<string, object> result = collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                                                  item =>
                                                                                                  Serializer.
                                                                                                      ToObjectSerialize
                                                                                                      <object>(item.Data));
			

            foreach (string key in keyList.Where(key => !result.ContainsKey(key)))
            {
                result.Add(key, null);
            }

            return result;
        }

        public IDictionary<string, object> GetRegex(string pattern)
        {
            MongoCollection collection = Collection;

            QueryComplete query = Query.Matches("_id", new BsonRegularExpression(pattern));

            return collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                    item =>
                                                                    Serializer.ToObjectSerialize<object>(item.Data));
        }

        public T Get<T>(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return default(T);

            return Serializer.ToObjectSerialize<T>(cacheItems.First().Data);
        }


        public long SizeAsKb(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);
            

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return 0;

            return cacheItems.First().Data.LongLength / 1024;
        }


        public long DecompressSizeAsKb(string key)
        {
            
            if (ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] != "1")
                return SizeAsKb(key);

            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);


            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return 0;
           
            return Compression.Decompress(cacheItems.First().Data).LongLength / 1024;
        }

        #region Nested type: CacheData

        [Serializable]
        private class CacheData
        {
            [BsonId]
            public string _id { get; set; }

            public byte[] Data { get; set; }
        }

        #endregion
    }
}