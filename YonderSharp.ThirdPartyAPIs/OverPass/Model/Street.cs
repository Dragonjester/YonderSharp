using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Model
{
    [DataContract]
    public class Street
    {
        [DataMember]
        public long StreetId { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string StreetName { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public int StreetSize { get; set; }

        //streetid, street, postcode, city , ST_Y(way::geometry), ST_X(way::geometry), streetsize
        public Street(long streetId, string street, string postcode, string city, double latitude, double longitude, int streetSize)
        {
            StreetId = streetId;
            City = city;
            PostCode = postcode;
            StreetName = street;
            Latitude = latitude;
            Longitude = longitude;
            StreetSize = StreetSize;
        }
    }
}
