using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace YonderSharp
{
    /// <summary>
    /// Helper class for zipping related operations
    /// </summary>
    public class Zipper
    {
        private static void CopyTo(Stream src, Stream dest)
        {
            var bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// returns the compressed base64 byte[] representation of the content
        /// </summary>
        public static string ZipBase64(string sr)
        {
            var data = Zip(sr);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// compresses the content
        /// </summary>
        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionLevel.Optimal))
                    {
                        CopyTo(msi, gs);
                    }

                    return mso.ToArray();
                }
            }
        }


        /// <summary>
        /// Turns the object into a JSON, zips it and stores it into a file
        /// </summary>
        public static void Zip(object obj, string outputPath)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            Zip(JsonConvert.SerializeObject(obj), outputPath);
        }

        /// <summary>
        /// Zips the string and stores it into a file
        /// </summary>
        public static void Zip(string content, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] resultToSave = Zip(content);
            File.WriteAllBytes(outputPath, resultToSave);
        }

        /// <summary>
        /// Unzips a file created by the Zip(object, path) method
        /// </summary>
        public static T Unzip<T>(string outputPath) 
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            string json = Unzip(File.ReadAllBytes(outputPath));
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Transformes the zipped bytes to their content string
        /// </summary>
        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
        }
    }
}
