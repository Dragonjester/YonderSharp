using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YonderSharp.Downloads;

namespace YonderSharp.Updater
{
    public class UpdateChecker
    {
        /// <summary>
        /// Checks if there is any new release avaiable
        /// </summary>
        /// <param name="urlOfIndex">Url to the json File </param>
        /// <returns>true if there is an update avaible. false if everything is up to date</returns>
        public static bool CheckForUpdate(string urlOfIndex)
        {
            if (string.IsNullOrWhiteSpace(urlOfIndex))
            {
                return false;
            }

            try
            {
                FileVersionInfo info = new FileVersionInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().GetName().Name,
                    Release = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                };

                IUrlDownloader downloader = new UrlDownloader("");

                string knownVersions = downloader.LoadFromUrl(urlOfIndex, true);
                if (string.IsNullOrWhiteSpace(knownVersions))
                {
                    return false;
                }

                List<FileVersionInfo> loadedVersions = new List<FileVersionInfo>();

                try
                {
                    loadedVersions.AddRange(JsonConvert.DeserializeObject<List<FileVersionInfo>>(knownVersions));
                }
                catch (Exception e)
                {
                    loadedVersions.Add(JsonConvert.DeserializeObject<FileVersionInfo>(knownVersions));
                }

                FileVersionInfo newestVersion = loadedVersions.FirstOrDefault(x => x.FileName.Equals(info.FileName, StringComparison.OrdinalIgnoreCase));
                return newestVersion != null && newestVersion.Release != info.Release;
            }
            catch (Exception e)
            {
                //maybe some logging in the future
            }

            return false;
        }
    }
}
