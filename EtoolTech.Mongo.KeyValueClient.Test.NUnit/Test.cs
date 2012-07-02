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
        public void TestCacheInsert()
        {
            var c = new Client();
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
            var c = new Client();
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
    }
}
