using System.Diagnostics;
using System.Runtime.Serialization;

namespace YonderSharp.WSG84
{
    [DataContract]
    [DebuggerDisplay("N{Latitude} E{Longitude}")]
    public class PointLatLng
    {
        public PointLatLng()
        {

        }

        /// <param name="latitude">Y</param>
        /// <param name="longitude">X</param>
        public PointLatLng(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Y
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// X
        /// </summary>
        public double Longitude { get; set; }
    }
}
