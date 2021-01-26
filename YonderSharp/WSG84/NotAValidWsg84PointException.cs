using System;

namespace YonderSharp.WSG84
{
    public class NotAValidWsg84PointException : Exception
    {

        public NotAValidWsg84PointException(PointLatLng point) : base($"N{point.Latitude} E{point.Longitude}")
        {

        }

        public NotAValidWsg84PointException(double latitude, double longitude) : base($"N{longitude} E{latitude}")
        {

        }
    }
}
