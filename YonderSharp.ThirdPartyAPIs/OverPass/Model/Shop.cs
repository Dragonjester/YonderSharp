using System.Runtime.Serialization;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Model
{
    [DataContract]
    public class Shop
    {
        [DataMember]
        public long ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string ShopType { get; set; }
        [DataMember]
        public string Amenity { get; set; }

        public Shop()
        {

        }

        public Shop(long id, double latitude, double longitude, string name, string shopType, string amenity)
        {
            ID = id;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            ShopType = shopType;
            Amenity = amenity;
        }
    }
}
