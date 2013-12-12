using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;

namespace EtoolTech.Mongo.KeyValueClient.Test.NUnit
{
    [Serializable]
    public class TestCacheData
    {
        public int FieldInt { get; set; }
        public DateTime FieldDateTime { get; set; }
        public float FieldFLoat { get; set; }
        public string FieldString { get; set; }
        public bool FieldBool { get; set; }
    }

}
