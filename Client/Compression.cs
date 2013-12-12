using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace EtoolTech.Mongo.KeyValueClient
{
    internal class Compression
    {
        private static readonly string CompresionMode = Config.Instance.CompressionMode;

        public static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                if (CompresionMode == "deflate")
                {
                    var ds = new DeflateStream(ms, CompressionMode.Compress);
                    ds.Write(data, 0, data.Length);
                    ds.Flush();
                    ds.Close();
                    return ms.ToArray();
                }
                else
                {
                    var ds = new GZipStream(ms, CompressionMode.Compress);
                    ds.Write(data, 0, data.Length);
                    ds.Flush();
                    ds.Close();
                    return ms.ToArray();
                }
            }
        }

        public static byte[] Decompress(byte[] data)
        {

            if (CompresionMode == "deflate")
                return DeflateDecompres(data);
            else
                return GzipDecompres(data);
        }

        private static byte[] DeflateDecompres(byte[] data)
        {
            const int bufferSize = 256;
            var tempArray = new byte[bufferSize];
            var tempList = new List<byte[]>();
            int count = 0, length = 0;

            using (var ms = new MemoryStream(data))
            {
                var ds = new DeflateStream(ms, CompressionMode.Decompress);


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
            }
            var retVal = new byte[length];

            count = 0;
            foreach (var temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }

            return retVal;
        }

        private static byte[] GzipDecompres(byte[] data)
        {
            const int bufferSize = 256;
            var tempArray = new byte[bufferSize];
            var tempList = new List<byte[]>();
            int count = 0, length = 0;

            using (var ms = new MemoryStream(data))
            {
                var ds = new GZipStream(ms, CompressionMode.Decompress);

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
            }
            var retVal = new byte[length];

            count = 0;
            foreach (var temp in tempList)
            {
                Array.Copy(temp, 0, retVal, count, temp.Length);
                count += temp.Length;
            }

            return retVal;
        }
    
    }
}