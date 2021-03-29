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
            if (!Directory.Exists(path)) { 
                Directory.CreateDirectory(path);
            }
        }

        public string LoadFromUrl(string Url)
        {
            return LoadFromUrl(Url, false);
        }

        Random rnd = new Random();
        public string LoadFromUrl(string Url, bool forceDownload)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            string result;

            var hash = GetUrlHash(Url);

            var fullPath = path + hash;
            if (!forceDownload)
            {
                if (!string.IsNullOrWhiteSpace(hash) && File.Exists(fullPath))
                {
                    try
                    {
                        return Zipper.Unzip(File.ReadAllBytes(fullPath));
                    }
                    catch (Exception e)
                    {
                        int i = 0;
                    }
                }
            }

            var myRequest = (HttpWebRequest)WebRequest.Create(Url);
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
                catch (Exception e)
                {
                    //TODO: fullPath ist u.U. bullshit, weil im Konstruktur bullshit übergeben wurde...
                }

                stopWatch.Stop();

                //don't bruteforce the server on purpose...
                Thread.Sleep(25 + rnd.Next(1, 50));
                return result;
            }
            catch (Exception e)
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
