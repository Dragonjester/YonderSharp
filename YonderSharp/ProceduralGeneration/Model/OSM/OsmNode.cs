using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.OSM
{
    [DataContract]
    [DebuggerDisplay("{Name} N{Latitude} E{Longitude}")]
    public class OsmNode
    {

        public OsmNode()
        {
            Tags = new List<KeyValuePair>();
        }

        public OsmNode(long iD, double latitude, double longitude, string name, List<KeyValuePair> tags)
        {
            ID = iD;
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            Tags = tags ?? new List<KeyValuePair>();
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
        public List<KeyValuePair> Tags { get; set; }
    }
}
