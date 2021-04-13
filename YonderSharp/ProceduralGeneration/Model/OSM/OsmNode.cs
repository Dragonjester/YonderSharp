using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using YonderSharp.Attributes;

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

        /// <summary>
        /// Unique ID of the Node
        /// </summary>
        [PrimaryKey]
        [DataMember]
        public long ID { get; set; }

        /// <summary>
        /// North (Y) component of the coordinate
        /// </summary>
        [DataMember]
        public double Latitude { get; set; }

        /// <summary>
        /// West (X) component of the coordinate
        /// </summary>
        [DataMember]
        public double Longitude { get; set; }

        /// <summary>
        /// Human readable name of the Node
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// K/V tags of the Node
        /// </summary>
        [DataMember]
        public List<KeyValuePair> Tags { get; set; }
    }
}
