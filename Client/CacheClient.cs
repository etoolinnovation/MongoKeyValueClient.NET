﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.Mongo.KeyValueClient
{
    public class CacheClient
    {
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["MongoCacheClient_ConnStr"];
        private static readonly string DataBaseName = ConfigurationManager.AppSettings["MongoCacheClient_Database"];
        private static readonly string CollectionName = ConfigurationManager.AppSettings["MongoCacheClient_Collection"];

        private static MongoCollection _col;


        public MongoDatabase Db
        {
            get { return Server.GetDatabase(DataBaseName); }
        }

        private MongoServer Server
        {
            get { return MongoServer.Create(ConnectionString); }
        }

        private MongoCollection Collection
        {
            get { return _col ?? (_col = Db.GetCollection(CollectionName)); }
        }

        #region ICacheClient Members

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
            collection.FindAndModify(query, null, Update.Set("Data", Serializer.ToByteArray(data)), false, true);
            return true;
        }

        public bool Remove(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);
            collection.Remove(query, SafeMode.True);
            return true;
        }

        public List<string> GetAllKeys()
        {
            MongoCollection collection = Collection;
            return collection.FindAllAs<CacheData>().SetFields("_id").ToList().Select(data => data._id).ToList();
        }

        public IDictionary<string, object> Get(List<string> keyList)
        {
            MongoCollection collection = Collection;

            QueryConditionList query = Query.In("_id", new BsonArray(keyList));

            IDictionary<string, object> result = collection.FindAs<CacheData>(query).ToDictionary(item => item._id,
                                                                                                  item =>
                                                                                                  Serializer.ToObjectSerialize
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
                                                                    item => Serializer.ToObjectSerialize<object>(item.Data));
        }

        #endregion

        public T Get<T>(string key)
        {
            MongoCollection collection = Collection;
            QueryComplete query = Query.EQ("_id", key);

            List<CacheData> cacheItems = collection.FindAs<CacheData>(query).ToList();

            if (!cacheItems.Any())
                return default(T);

            return Serializer.ToObjectSerialize<T>(cacheItems.First().Data);
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