using System.Configuration;

namespace EtoolTech.Mongo.KeyValueClient
{
    public class Config
    {
        private static Config _config = null;

        public static Config Instance
        {
            get { return _config ?? (_config = new Config()); }
        }

        public Config()
        {
            //<add key="MongoKeyValueClient_ConnStr" value="mongodb://127.0.0.1"/>
            //<add key="MongoKeyValueClient_Database" value="MongoKeyValueTest"/>
            //<add key="MongoKeyValueClient_Collection" value="CacheData"/>
            //<add key="MongoKeyValueClient_CompressionEnabled" value="1"/>
            //<add key="MongoKeyValueClient_SerializationMode" value="json"/>
            //<add key="MongoKeyValueClient_CompressionMode" value="gzip"/>
            //<add key ="MongoKeyValueClient_ShowSizes" value="0"/>    
            this.ConnStr = ConfigurationManager.AppSettings["MongoKeyValueClient_ConnStr"];
            this.Database = ConfigurationManager.AppSettings["MongoKeyValueClient_Database"];
            this.Collection = ConfigurationManager.AppSettings["MongoKeyValueClient_Collection"];
            this.PrefixCollection = ConfigurationManager.AppSettings["PrefixCollection"];
            this.CompresionEnabled = ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionEnabled"] == "1";            
            this.CompressionMode = ConfigurationManager.AppSettings["MongoKeyValueClient_CompressionMode"];
            this.ShowSizes = ConfigurationManager.AppSettings["MongoKeyValueClient_ShowSizes"] == "1";
        }

        public string ConnStr { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
        public string PrefixCollection { get; set; }
        public bool CompresionEnabled { get; set; }        
        public string CompressionMode { get; set; }
        public bool ShowSizes { get; set; }



    }
}
