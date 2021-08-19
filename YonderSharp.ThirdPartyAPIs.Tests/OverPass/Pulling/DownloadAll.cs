using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YonderSharp.ThirdPartyAPIs.OverPass.Model;
using YonderSharp.ThirdPartyAPIs.OverPass.Pulling;

namespace YonderSharp.ThirdPartyAPIs.Tests.OverPass.Pulling
{
    public class DownloadAll
    {
        CityPuller cityPuller = new CityPuller();
        ShopPuller shopPuller = new ShopPuller();

        private const string cacheDirectory = "D:\\overpasscache";
        private string pathToHumanInhabitatedLatLngs = "D:\\overpasscache\\human inhabitated latLongs.json";
        private int countTotal = 0;
        List<int[]> latLongs = new List<int[]>();
        private object locker = new object();



        [Test]
        public void DownloadCitiesAndShops()
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            //rest of world
            for (int latitude = -89; latitude < 90; latitude++)
            {
                for (int longitude = -179; longitude < 180; longitude++)
                {
                    Execute(latitude, longitude);
                }
            }

            string output = JsonConvert.SerializeObject(latLongs);
            File.WriteAllText(pathToHumanInhabitatedLatLngs, output);
            int i = 0;
        }

        [Test]
        public void MergeHumanInhabitated()
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            var latLngs = JsonConvert.DeserializeObject<List<int[]>>(File.ReadAllText(pathToHumanInhabitatedLatLngs));
            Dictionary<long, Shop> shops = new Dictionary<long, Shop>();
            Dictionary<long, City> cities = new Dictionary<long, City>();
            int count = 0;
            foreach (int[] latLng in latLngs)
            {
                count++;
                var loadedShops = shopPuller.Pull(latLng[0], latLng[1], latLng[0], latLng[1], 0);
                var loadedCities = cityPuller.Pull(latLng[0], latLng[1], latLng[0], latLng[1], 0);
                foreach (var shop in loadedShops)
                {
                    shops[shop.ID] = shop;
                }

                foreach (var city in loadedCities)
                {
                    cities[city.ID] = city;
                }
            }

       
        }


        private void Execute(int latitude, int longitude)
        {
            int atStart = Directory.GetFiles(cacheDirectory).Length;
            countTotal++;
            bool hasCity = false;
            bool hasShop = false;

            var loopResult = Parallel.ForEach(new[] { 1, 2 }, i =>
            {
                try
                {
                    if (i == 1)
                    {
                        hasCity = cityPuller.Pull(latitude, longitude, latitude, longitude, 0).Any(x => !string.IsNullOrWhiteSpace(x.Name));
                    }
                    else if (i == 2)
                    {
                        hasShop = shopPuller.Pull(latitude, longitude, latitude, longitude, 0).Any(x => !string.IsNullOrWhiteSpace(x.Name));
                    }
                }
                catch (Exception e)
                {
                    int xyz = 0;
                }
            });

            while (!loopResult.IsCompleted)
            {
                Thread.Sleep(10);
            }

            if (hasCity || hasShop)
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
