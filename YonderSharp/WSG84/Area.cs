using System.Runtime.Serialization;

namespace YonderSharp.WSG84
{
    [DataContract]
    public class Area
    {
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


        [DataMember]
        public double XStart { get; set; }
        [DataMember]
        public double XEnd { get; set; }
        [DataMember]
        public double YStart { get; set; }
        [DataMember]
        public double YEnd { get; set; }


        public double XCenter { get { return (XStart + XEnd) / 2; } }
        public double YCenter { get { return (YStart + YEnd) / 2; } }
        public PointLatLng TopLeftCorner { get { return new PointLatLng(YStart, XStart); } }
        public PointLatLng BottomRightCorner { get { return new PointLatLng(YEnd, XEnd); } }
        public PointLatLng Center { get { return new PointLatLng(YCenter, XCenter); } }
    }
}
