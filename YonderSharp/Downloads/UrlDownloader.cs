using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace YonderSharp.Downloads
{
    public class UrlDownloader : IUrlDownloader
    {
        string path { get; set; }

        public UrlDownloader(string pathToCacheIn)
        {
            path = pathToCacheIn;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Load the htmlbody from the web
        /// </summary>
        /// <param name="url">url of the ressource</param>
        public string LoadFromUrl(string url)
        {
            return LoadFromUrl(url, false);
        }

        Random rnd = new Random();

        /// <summary>
        /// Load the htmlbody from the web
        /// </summary>
        /// <param name="url">url of the ressource</param>
        /// <param name="forceDownload">Doesnt try to read from the local cache if <b>TRUE</b></param>
        /// <returns>the string represenation of the htmlBody that has been downloaded</returns>
        public string LoadFromUrl(string url, bool forceDownload)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            string result;

            var hash = GetUrlHash(url);

            var fullPath = path + hash;
            if (!forceDownload)
            {
                if (!string.IsNullOrWhiteSpace(hash) && File.Exists(fullPath))
                {
                    try
                    {
                        return Zipper.Unzip(File.ReadAllBytes(fullPath));
                    }
                    catch
                    {
                        Debugger.Break();
                    }
                }
            }

            var myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";

            try
            {
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                try
                {
                    File.WriteAllBytes(fullPath, Zipper.Zip(result));
                }
                catch
                {
                    //TODO: fullPath ist u.U. bullshit, weil im Konstruktur bullshit übergeben wurde...
                }

                stopWatch.Stop();

                //don't bruteforce the server on purpose...
                Thread.Sleep(25 + rnd.Next(1, 50));
                return result;
            }
            catch 
            {
                return "";
            }
        }

        private string GetUrlHash(string url)
        {
            return url.Replace("/", " ").Replace("_", " ").Replace("\\", " ").Replace(":", " ").Replace(".", " ").Replace("-", " ").Replace("  ", " ") + ".zip";
        }
    }
}
