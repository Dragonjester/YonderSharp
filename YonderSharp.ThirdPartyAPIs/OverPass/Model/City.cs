using System.Diagnostics;
using System.Runtime.Serialization;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Model
{
    [DebuggerDisplay("{Name} N{Latitude} E{Longitude}")]
    [DataContract]
    public class City
    {
        public City()
        {

        }
        public City(long iD, double latitude, double longitude, string name, CityType type)
        {
            ID = iD;
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            Type = type;
        }

        [DataMember]
        public long ID { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public CityType Type { get; set; }
    }
}
