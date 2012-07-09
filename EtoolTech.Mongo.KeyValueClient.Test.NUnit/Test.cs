using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace EtoolTech.Mongo.KeyValueClient.Test.NUnit
{
    [TestFixture]
    public class Test
    {

        [Test]
        public void TestGetForWrite()
        {
            var c = new Client();
            foreach (string key in c.GetAllKeys())
            {
                c.Remove(key);
            }

          
            c.Add("100000", 100000);
            c.Add("200000", 200000);
            c.Add("300000", 300000);

            List<string> keys = c.GetAllKeys();

            Assert.AreEqual(3, keys.Count);

            foreach (string key in keys)
            {
                var value = c.GetForWrite<int>(key);
                Assert.AreEqual(key, value.ToString());                

                c.Remove(key);
            }


            keys = c.GetAllKeys();

            Assert.AreEqual(0, keys.Count);
        }

         [Test]
        public void TestCacheAloneInsert()
         {
             var c = new Client();
             foreach (string key in c.GetAllKeys())
             {
                 c.Remove(key);
             }

             c.Add("Key", 100000);

             List<string> keys = c.GetAllKeys();

             Assert.AreEqual(1, keys.Count);

             foreach (string key in keys)
             {
                 var value = c.Get<int>(key);
                 Assert.AreEqual(key, "Key");
                 Assert.AreEqual(100000, value);

                 c.Remove(key);
             }


             keys = c.GetAllKeys();

             Assert.AreEqual(0, keys.Count);
         }

        [Test]
        public void TestCacheInsert()
        {
            var c = new Client();
            foreach (string key in c.GetAllKeys())
            {
                c.Remove(key);
            }

            System.Threading.Tasks.Parallel.For(0, 10000, index => c.Add(index.ToString(),index));

            List<string> keys = c.GetAllKeys();

            Assert.AreEqual(10000, keys.Count);

            foreach (string key in keys)
            {
                var value = c.Get<int>(key);
                Assert.AreEqual(key, value.ToString());

                c.Remove(key);
            }


            keys = c.GetAllKeys();

            Assert.AreEqual(0, keys.Count);

        }


		 [Test]
        public void TestKeySize()
        {
			try
			{
				List<string> keys = new List<string>(5000010);
            	System.Threading.Tasks.Parallel.For(0, 5000000, index => keys.Add("THIS_IS_A_CACHE_KAYE" + index.ToString()));

				var c = new Client();
				c.Get(keys);
			}
			catch(System.IO.FileFormatException)
			{
				//ok
			}
        }
    }
}
