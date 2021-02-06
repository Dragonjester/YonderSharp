using System;

namespace YonderSharp.WSG84
{
    /// <summary>
    /// Either you've done something stupid, or something has gone terrible wrong somewhere...
    /// </summary>
    public class NotAValidWsg84PointException : Exception
    {
        /// <summary/>
        public NotAValidWsg84PointException(PointLatLng point) : base($"N{point.Latitude} E{point.Longitude}")
        {

        }
        /// <summary/>
        public NotAValidWsg84PointException(double latitude, double longitude) : base($"N{longitude} E{latitude}")
        {

        }
    }
}
