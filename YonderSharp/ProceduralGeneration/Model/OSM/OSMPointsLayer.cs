using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.OSM
{
    [DataContract]
    public class OSMPointsLayer : Layer
    {

        public OSMPointsLayer()
        {
            ID = Guid.NewGuid();
            TypeOfLayer = LayerType.OSMPoints;
            NodeTags = new List<KeyValuePair>();
        }

        /// <summary>
        /// city = city
        /// </summary>
        [DataMember]
        public bool CityTypeCity { get; set; }

        /// <summary>
        /// city = town
        /// </summary>
        [DataMember]
        public bool CityTypeTown { get; set; }

        /// <summary>
        /// city = suburb
        /// </summary>
        [DataMember]
        public bool CityTypeSuburb { get; set; }

        /// <summary>
        /// city = village
        /// </summary>
        [DataMember]
        public bool CityTypeVillage { get; set; }

        /// <summary>
        /// city = neighbourhood
        /// </summary>
        [DataMember]
        public bool CityTypeNeighbourhood { get; set; }

        /// <summary>
        /// i.e. {"concentration_camp", "nazism"}
        /// </summary>
        [DataMember]
        public List<KeyValuePair> NodeTags { get; set; }

        [DataMember]
        public int MaxDistanceToCity { get; set; }

        [DataMember]
        public int MaxDistaToTag { get; set; }
    }
}
