using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.OSM
{
    [DataContract]
    public class KeyValuePair
    {
        public KeyValuePair()
        {

        }
        public KeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
