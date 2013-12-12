using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace EtoolTech.Mongo.KeyValueClient.Test.NUnit
{
    [TestFixture]
    public class Test
    {

        [Test]
        public void TestJson()
        {
            var c = new Client();
            c.RemoveAll();

            var listEmpty = c.Get<List<TestCacheData>>("22");

            var t = new TestCacheData();
            t.FieldString = "Hola";
            t.FieldInt = 25;
            t.FieldFLoat = float.Parse("100,00");
            t.FieldDateTime = DateTime.Now;
            t.FieldBool = false;

            c.Add("HOLA_1", t);

            var t2 = c.Get<TestCacheData>("HOLA_1");

            string tstring = c.GetAsString("HOLA_1");
            

            Assert.AreEqual(t.FieldString, t2.FieldString);

            var list = new List<TestCacheData>();
            for (int i = 0; i < 100; i++)
            {
                var t3 = new TestCacheData();
                t3.FieldString = "Hola";
                t3.FieldInt = 25;
                t3.FieldFLoat = float.Parse("100,00");
                t3.FieldDateTime = DateTime.Now;
                t3.FieldBool = false;
                list.Add(t3);
            }

            c.Add("HOLA_2", list);            

            var list2 = c.Get<List<TestCacheData>>("HOLA_2");
        }

        [Test]
        public void TestGetForWrite()
        {
            var c = new Client();
            c.RemoveAll();

          
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

            System.Threading.Thread.Sleep(5000);

            keys = c.GetAllKeys();

            Assert.AreEqual(0, keys.Count);
        }

         [Test]
        public void TestCacheAloneInsert()
         {
             var c = new Client();
             c.RemoveAll();

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

             System.Threading.Thread.Sleep(5000);

             keys = c.GetAllKeys();

             Assert.AreEqual(0, keys.Count);
         }

        [Test]
        public void TestCacheInsert()
        {
            var c = new Client();
            c.RemoveAll();
      
            System.Threading.Tasks.Parallel.For(0, 10000, index => c.Add(index.ToString(),index));

            List<string> keys = c.GetAllKeys();

            Assert.AreEqual(10000, keys.Count);

            foreach (string key in keys)
            {
                var value = c.Get<int>(key);
                Assert.AreEqual(key, value.ToString());

                c.Remove(key);
            }


            System.Threading.Thread.Sleep(5000);
            
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
				c.Get<string>(keys);
			}
			catch(Exception e)
			{
				Assert.AreEqual(e.GetBaseException().GetType().ToString(), typeof(System.IO.FileFormatException).ToString());
			}
        }
    }
}
