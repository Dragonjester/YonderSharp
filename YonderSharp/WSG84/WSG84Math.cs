using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp.WSG84
{
    public class WSG84Math
    {
        public static double GetDistanceInMeters(PointLatLng a, double latitudeB, double longitudeB)
        {
            return GetDistanceInMeters(a.Latitude, a.Longitude, latitudeB, longitudeB);
        }

        public static double GetDistanceInMeters(PointLatLng a, PointLatLng b)
        {
            return GetDistanceInMeters(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public static double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            return GetDistanceInKilometers(lat1, lon1, lat2, lon2) * 1000;
        }

        /// <summary>
        /// Wicked math that determines the distance between 2 points
        /// </summary>
        public static double GetDistanceInKilometers(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = DegreeToRad(lat2 - lat1);
            var dLon = DegreeToRad(lon2 - lon1);
            lat1 = DegreeToRad(lat1);
            lat2 = DegreeToRad(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        /// <summary>
        /// Go read a math book
        /// </summary>
        public static double DegreeToRad(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Go read a math book
        /// </summary>
        public static double RadToDegree(double rad)
        {
            return rad * 180 / Math.PI;
        }


        /// <summary>
        /// Returns an area which contains the circle created by the given parameters
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="maxDistanceInMeters"></param>
        /// <returns></returns>
        public static Area GetAreaFor(PointLatLng centerPoint, double distanceInMeters)
        {
            //increase distance by 12 (picked by trying around), to ensure that the area does contain the given circle in the case of rounding issues
            double distance = distanceInMeters + 12;

            //if you want to know the math behind this calculation, remember that a² + b² = c² and draw a rectangle around a circle
            //1.41 = sqrt(2)
            PointLatLng topLeft = GetDestinationPoint(centerPoint, 315, distance * 1.41);
            PointLatLng bottomRight = GetDestinationPoint(centerPoint, 135, distance * 1.41);

            return new Area(topLeft, bottomRight);
        }

        /// <summary>
        /// Returns an area which contains the circle created by the given parameters
        /// </summary>
        /// <param name="latitudeCenter">Y</param>
        /// <param name="longitudeCenter">X</param>
        /// <param name="maxDistanceInMeters"></param>
        /// <returns></returns>
        public static Area GetAreaFor(double latitudeCenter, double longitudeCenter, double distanceInMeters)
        {
            return GetAreaFor(new PointLatLng(latitudeCenter, longitudeCenter), distanceInMeters);
        }

        /// <summary>
        /// Calculates the end-point from a given source at a given range (meters) and bearing (degrees).
        /// This methods uses simple geometry equations to calculate the end-point.
        /// </summary>
        /// <param name="source">Point of origin</param>
        /// <param name="bearing">Bearing in degrees</param>
        /// <param name="distanceInMeters">Range in meters</param>
        /// <returns>End-point from the source given the desired range and bearing.</returns>
        public static PointLatLng GetDestinationPoint(PointLatLng source, double bearing, double distanceInMeters)
        {
            distanceInMeters = distanceInMeters / 6371000;
            bearing = DegreeToRad(bearing);

            double lat1 = DegreeToRad(source.Latitude);
            double lon1 = DegreeToRad(source.Longitude);

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distanceInMeters) +
                    Math.Cos(lat1) * Math.Sin(distanceInMeters) * Math.Cos(bearing));

            double dlon = Math.Atan2(
                   Math.Sin(bearing) * Math.Sin(distanceInMeters) * Math.Cos(lat1),
                   Math.Cos(distanceInMeters) - Math.Sin(lat1) * Math.Sin(lat2));

            double lon = ((lon1 + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

            return new PointLatLng(RadToDegree(lat2), RadToDegree(lon));
        }

     
        /// <param name="latitude">Y</param>
        /// <param name="longitude">X</param>        
        internal static bool IsValidWSG84Point(PointLatLng point)
        {
            return IsValidWSG84Point(point.Latitude, point.Longitude);
        }

        /// <param name="latitude">Y</param>
        /// <param name="longitude">X</param>        
        public static bool IsValidWSG84Point(double latitude, double longitude)
        {
            return Math.Abs(latitude) <= 89.9999 && Math.Abs(longitude) <= 179.9999;
        }


        public static bool IsPointWithinArea(Area area, PointLatLng point)
        {
            bool isAbove = area.TopLeftCorner.Latitude < point.Latitude;
            bool isBelow = area.BottomRightCorner.Latitude > point.Latitude;
            bool isLeft = area.TopLeftCorner.Longitude > point.Longitude;
            bool isRight = area.BottomRightCorner.Longitude < point.Longitude;

            return !(isAbove || isBelow || isLeft || isRight);
        }
    }
}
