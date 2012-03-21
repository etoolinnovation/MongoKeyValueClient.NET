using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class Compression
    {
        internal static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                var ds = new DeflateStream(ms, CompressionMode.Compress);
                ds.Write(data, 0, data.Length);
                ds.Flush();
                ds.Close();
                return ms.ToArray();
            }
        }

        internal static byte[] Decompress(byte[] data)
        {
            const int bufferSize = 256;
            var tempArray = new byte[bufferSize];
            var tempList = new List<byte[]>();
            int count = 0, length = 0;

            DeflateStream ds;
            using (var ms = new MemoryStream(data))
            {
                ds = new DeflateStream(ms, CompressionMode.Decompress);
            }

            while ((count = ds.Read(tempArray, 0, bufferSize)) > 0)
            {
                if (count == bufferSize)
                {
                    tempList.Add(tempArray);
                    tempArray = new byte[bufferSize];
                }
                else
                {
                    var temp = new byte[count];
                    Array.Copy(tempArray, 0, temp, 0, count);
                    tempList.Add(temp);
                }
                length += count;
            }

            var retVal = new byte[length];

            count = 0;
            foreach (byte[] temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }

            return retVal;
        }
    }
}
