using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;
using KeyValuePair = YonderSharp.ProceduralGeneration.Model.OSM.KeyValuePair;

namespace YonderSharp.Test.WSG84Tests
{
    [TestFixture]
    public class GeoSorterTests
    {
        private OsmNode[] GetTestValues()
        {
            var result = new List<OsmNode>();

            OsmNode one = new OsmNode();
            one.Latitude = 0;
            one.Longitude = 0;
            one.Name = nameof(one);
            one.Tags.Add(new KeyValuePair("place", "city"));
            one.Tags.Add(new KeyValuePair("amenity", "statue"));
            result.Add(one);

            OsmNode two = new OsmNode();
            two.Latitude = 1;
            two.Longitude = 1;
            two.Name = nameof(two);
            two.Tags.Add(new KeyValuePair("place", "city"));
            two.Tags.Add(new KeyValuePair("amenity", "statue"));
            result.Add(two);

            OsmNode three = new OsmNode();
            three.Latitude = 3;
            three.Longitude = 3;
            three.Name = nameof(three);
            three.Tags.Add(new KeyValuePair("place", "village"));
            three.Tags.Add(new KeyValuePair("amenity", "statue"));
            result.Add(three);

            OsmNode four = new OsmNode();
            four.Latitude = 4;
            four.Longitude = 4;
            four.Name = nameof(four);
            four.Tags.Add(new KeyValuePair("place", "village"));
            four.Tags.Add(new KeyValuePair("amenity", "statue"));
            result.Add(four);

            return result.ToArray();
        }

        [Test]
        public void SortByDistanceTest()
        {
            var nodes = GetTestValues();
            var point = new PointLatLng(3, 3);

            var sortedPoints = GeoSorter.SortByDistance(nodes, point);

            Assert.AreEqual(sortedPoints[0], nodes[2]);
            Assert.AreEqual(sortedPoints[1], nodes[3]);
            Assert.AreEqual(sortedPoints[2], nodes[1]);
            Assert.AreEqual(sortedPoints[3], nodes[0]);


            sortedPoints = GeoSorter.SortByDistance(nodes, point, false);

            Assert.AreEqual(sortedPoints[3], nodes[2]);
            Assert.AreEqual(sortedPoints[2], nodes[3]);
            Assert.AreEqual(sortedPoints[1], nodes[1]);
            Assert.AreEqual(sortedPoints[0], nodes[0]);
        }

        [Test]
        public void MakeKeyValueUniqueToClosestTest()
        {
            var nodes = GetTestValues();
            var point = new PointLatLng(3, 3);

            var uniquedPoints = GeoSorter.MakeKeyValueUniqueToClosest(nodes, point, "place");
            Assert.AreEqual(2, uniquedPoints.Length);

            Assert.IsTrue(uniquedPoints.Contains(nodes[1]));
            Assert.IsTrue(uniquedPoints.Contains(nodes[2]));

        }
    }
}
