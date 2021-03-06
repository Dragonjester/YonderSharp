﻿using System.Runtime.Serialization;

namespace YonderSharp.WSG84
{
    /// <summary>
    /// Represents an rectacle, using the WSG84 floating point notation
    /// </summary>
    [DataContract]
    public class Area
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Area()
        {

        }

        public Area(double north, double east, double south, double west)
        {
            YStart = north;
            XStart = west;
            YEnd = south;
            XEnd = east;
        }

        public Area(PointLatLng topLeftCorner, PointLatLng bottomRightCorner)
        {
            YStart = topLeftCorner.Latitude;
            XStart = topLeftCorner.Longitude;
            YEnd = bottomRightCorner.Latitude;
            XEnd = bottomRightCorner.Longitude;
        }

        #region datamembers
        [DataMember]
        public double XStart { get; set; }

        [DataMember]
        public double XEnd { get; set; }
        [DataMember]
        public double YStart { get; set; }
        [DataMember]
        public double YEnd { get; set; }
        #endregion datamembers

        #region derived values
        public double XCenter { get { return (XStart + XEnd) / 2; } }
        public double YCenter { get { return (YStart + YEnd) / 2; } }
        public PointLatLng TopLeftCorner { get { return new PointLatLng(YStart, XStart); } }
        public PointLatLng BottomRightCorner { get { return new PointLatLng(YEnd, XEnd); } }
        public PointLatLng Center { get { return new PointLatLng(YCenter, XCenter); } }
        #endregion derived values
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
