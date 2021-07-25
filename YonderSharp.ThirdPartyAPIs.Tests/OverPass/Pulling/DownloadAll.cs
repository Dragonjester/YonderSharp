using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YonderSharp.ThirdPartyAPIs.OverPass.Pulling;

namespace YonderSharp.ThirdPartyAPIs.Tests.OverPass.Pulling
{
    public class DownloadAll
    {
        CityPuller cityPuller = new CityPuller();
        ShopPuller shopPuller = new ShopPuller();

        private const string cacheDirectory = "D:\\overpasscache";
        private int countTotal = 0;
        List<int[]> latLongs = new List<int[]>();

        [Test]
        public void DownloadCitiesAndShops()
        {
            //rest of world
            for (int latitude = -89; latitude < 89; latitude++)
            {
                for (int longitude = -179; longitude < 179; longitude++)
                {
                    Execute(latitude, longitude);
                }
            }

            string output = JsonConvert.SerializeObject(latLongs);
            File.WriteAllText(cacheDirectory + "\\human inhabitated latLongs.json", output);
        }

        private object locker = new object();
        private void Execute(int latitude, int longitude)
        {
            int atStart = Directory.GetFiles(cacheDirectory).Length;
            countTotal++;
            int countCity = 0;
            int countShop = 0;

            var loopResult = Parallel.ForEach(new[] { 1, 2 }, i =>
            {
                try
                {
                    if (i == 1)
                    {
                        countCity = cityPuller.Pull(latitude, longitude, latitude, longitude, 0).ToArray().Length;
                    }
                    else if (i == 2)
                    {
                        countShop = shopPuller.Pull(latitude, longitude, latitude, longitude, 0).ToArray().Length;
                    }
                }
                catch (Exception e)
                {
                    int xyz = 0;
                }
            });

            while (!loopResult.IsCompleted)
            {
                Thread.Sleep(100);
            }

            if (countCity + countShop > 0)
            {
                lock (locker)
                {
                    latLongs.Add(new[] { latitude, longitude });
                }
            }


            int atEnd = Directory.GetFiles(cacheDirectory).Length;
            if (atStart != atEnd)
            {
                //maybe don't nuke the server with lots and lots of requests
                Thread.Sleep(2500);
            }
        }
    }
}
