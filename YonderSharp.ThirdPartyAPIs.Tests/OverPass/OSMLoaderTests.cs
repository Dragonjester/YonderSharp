using NUnit.Framework;
using System.Linq;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.ThirdPartyAPIs.OverPass;
using YonderSharp.WSG84;

namespace YonderSharp.ThirdPartyAPIs.Tests.OverPass
{
    public class OSMLoaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetOsmNodeTest()
        {
            var client = new OverpassApi();
            var result = client.GetOsmNode(3657889771);
            Assert.That(result != null);
            Assert.That("Spiegel" == result.Name);
            Assert.That(1 == result.Tags.Count);
            Assert.That("amenity" == result.Tags[0].Key);
            Assert.That("bar" == result.Tags[0].Value);
        }

        [Test]
        public void GetSpecificCity()
        {

            var client = new OverpassApi();

            string query = @"[out:csv(::id, ::lat, ::lon, name);
            // gather results

            node[place=city][""name""=""Calden""];
            node[place=town][""name""=""Calden""];
            node[place=village][""name""=""Calden""];

            // print results
            out body;";

            string result = client.GetOverPassData(query);

            Assert.That(result.Contains("51.4081597"));
            Assert.That(result.Contains("9.4023531"));
        }


        [Test]
        public void TestGetCities()
        {
            var client = new OverpassApi();

            OSMPointsLayer layer = new OSMPointsLayer();
            layer.CityTypeNeighbourhood = true;
            layer.CityTypeCity = true;
            layer.CityTypeSuburb = true;
            layer.CityTypeTown = true;
            layer.CityTypeVillage = true;

            layer.MaxDistanceToCity = 40000;
            layer.MaxDistaToTag = 2000;

            var result = client.GetOsmNodes(layer, new PointLatLng(51.4894273, 9.1421818), 10000);

            //if the city isn't found, it throws an exception -> Test failed :-)
            result.First(x => x.Name.Equals("Warburg"));
            result.First(x => x.Name.Equals("Nörde"));
            result.First(x => x.Name.Equals("Hohenwepel"));
            result.First(x => x.Name.Equals("Menne"));
        }
    }
}