using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace YonderSharp.Downloads
{
    public class UrlDownloader : IUrlDownloader
    {
        string path { get; set; }
        bool doZip { get; set; }

        public UrlDownloader(string pathToCacheIn, long maxAgeOfCacheInMs = 30l * 24l * 60l * 60l * 1000l)
        {
            path = pathToCacheIn;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists)
                    {
                        var age = DateTime.UtcNow - fileInfo.CreationTimeUtc;
                        if (age.TotalMilliseconds > maxAgeOfCacheInMs)
                        {
                            fileInfo.Delete();
                        }
                    }
                }
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

            var hash = GetFileName(url);

            var fullPath = path + hash;
            if (!forceDownload)
            {
                if (!string.IsNullOrWhiteSpace(hash) && File.Exists(fullPath))
                {
                    try
                    {
                        if (doZip)
                        {
                            return Zipper.Unzip(File.ReadAllBytes(fullPath));
                        }
                        else
                        {
                            return File.ReadAllText(fullPath);
                        }
                    }
                    catch(Exception e)
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
                    if (doZip)
                    {
                        File.WriteAllBytes(fullPath + ".zip", Zipper.Zip(result));

                    }
                    else
                    {
                        File.WriteAllText(fullPath, result);
                    }

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

        private string GetFileName(string url)
        {
            return url.Split("/", StringSplitOptions.RemoveEmptyEntries).Last();
        }
    }
}
