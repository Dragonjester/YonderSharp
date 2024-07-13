using NUnit.Framework;
using System;
using YonderSharp.WSG84;
using NUnit.Framework.Legacy;

namespace YonderSharp.Test.WSG84Tests
{
    [TestFixture]
    public class HaversineFormulaTests
    {

        [Test]
        public void DistanceInMeterTests()
        {
            Tuple<double, double, double, double, int>[] meterEntries = new Tuple<double, double, double, double, int>[]
            {
                new Tuple<double, double, double, double, int>(0,0,1,1, 157293)
            };

            foreach (var entry in meterEntries)
            {
                double distanceInMeters = WSG84Math.GetDistanceInMeters(entry.Item1, entry.Item2, entry.Item3, entry.Item4);
                ClassicAssert.AreEqual((int)distanceInMeters, entry.Item5);
            }
        }

        [Test]
        public void DistanceInKmTests()
        {
            Tuple<double, double, double, double, int>[] testEntries = new Tuple<double, double, double, double, int>[]
            {
                new Tuple<double, double, double, double, int>(0, 0, 1, 1, 157)
            };

            foreach (var entry in testEntries)
            {
                double distanceInKilometers = WSG84Math.GetDistanceInKilometers(entry.Item1, entry.Item2, entry.Item3, entry.Item4);
                ClassicAssert.AreEqual((int)distanceInKilometers, entry.Item5);
            }
        }

        [Test]
        public void GetDestinationPointTests()
        {
            PointLatLng point = new PointLatLng(0, 0);
            PointLatLng movedPoint = WSG84Math.GetDestinationPoint(point, 45, 157000);
            var latitudeError = Math.Abs(movedPoint.Latitude - 1);
            var longitudeError = Math.Abs(movedPoint.Longitude - 1);

            ClassicAssert.IsTrue(latitudeError < 0.002);
            ClassicAssert.IsTrue(longitudeError < 0.002);
        }

        [Test]
        public void TestGetAreaFor()
        {
            var distance = 10000.0;
            Area result = WSG84Math.GetAreaFor(51, 7, distance);

            var distanceTopLeftToCenter = WSG84Math.GetDistanceInMeters(result.Center, result.TopLeftCorner);
            var distanceBottomRightToCenter = WSG84Math.GetDistanceInMeters(result.Center, result.BottomRightCorner);
            var distanceTopLeftToBorromRight = WSG84Math.GetDistanceInMeters(result.TopLeftCorner, result.BottomRightCorner);

            //if you don't know why, draw the smallest possible rectangle which contains a circle with the radius r=1 
            //and try to figure out the distance between the center without a ruler. Math only!
            //hint: a² + b² = c²
            var expectedDistance = distance * Math.Sqrt(2);

            //calculating with WSG84 is messy since we aren't living on an euclidean-2D-plane
            var epsilon = 10;

            ClassicAssert.IsTrue(distanceBottomRightToCenter - expectedDistance < epsilon);
            ClassicAssert.IsTrue(distanceTopLeftToCenter - expectedDistance < epsilon);
            ClassicAssert.IsTrue(distanceTopLeftToBorromRight - 2 * expectedDistance < epsilon);
        }

        [Test]
        public void PointIncludedInAreaTest()
        {
            Area area = WSG84Math.GetAreaFor(0, 0, 100000);

            PointLatLng center = new PointLatLng(0, 0);
            ClassicAssert.IsTrue(WSG84Math.IsPointWithinArea(area, center));

            PointLatLng toLeft = new PointLatLng(0, -1);
            ClassicAssert.IsFalse(WSG84Math.IsPointWithinArea(area, toLeft));

            PointLatLng toRight = new PointLatLng(0, 1);
            ClassicAssert.IsFalse(WSG84Math.IsPointWithinArea(area, toRight));

            PointLatLng above = new PointLatLng(1, 0);
            ClassicAssert.IsFalse(WSG84Math.IsPointWithinArea(area, above));

            PointLatLng below = new PointLatLng(-1, -1);
            ClassicAssert.IsFalse(WSG84Math.IsPointWithinArea(area, below));
        }


    }
}
