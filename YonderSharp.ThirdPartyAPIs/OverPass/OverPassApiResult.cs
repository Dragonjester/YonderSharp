using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{

    public partial class OverPassApiResult
    {
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public double? Version { get; set; }

        [JsonProperty("generator", NullValueHandling = NullValueHandling.Ignore)]
        public string Generator { get; set; }

        [JsonProperty("osm3s", NullValueHandling = NullValueHandling.Ignore)]
        public Osm3S Osm3S { get; set; }

        [JsonProperty("elements", NullValueHandling = NullValueHandling.Ignore)]
        public Element[] Elements { get; set; }
    }

    public partial class Element
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public double Lat { get; set; }

        [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
        public double Lon { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Tags { get; set; }
    }

    public partial class Tags
    {
        [JsonProperty("man_made", NullValueHandling = NullValueHandling.Ignore)]
        public string ManMade { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("tower:type", NullValueHandling = NullValueHandling.Ignore)]
        public string TowerType { get; set; }

        [JsonProperty("historic", NullValueHandling = NullValueHandling.Ignore)]
        public string Historic { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Image { get; set; }

        [JsonProperty("wikidata", NullValueHandling = NullValueHandling.Ignore)]
        public string Wikidata { get; set; }

        [JsonProperty("wikipedia", NullValueHandling = NullValueHandling.Ignore)]
        public string Wikipedia { get; set; }
    }

    public partial class Osm3S
    {
        [JsonProperty("timestamp_osm_base", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? TimestampOsmBase { get; set; }

        [JsonProperty("copyright", NullValueHandling = NullValueHandling.Ignore)]
        public string Copyright { get; set; }
    }
}
