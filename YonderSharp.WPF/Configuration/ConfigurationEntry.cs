using System.Runtime.Serialization;
using YonderSharp.Attributes;

namespace YonderSharp.WPF.Configuration
{
    [DataContract]
    public class ConfigurationEntry
    {
        public ConfigurationEntry()
        {

        }

        public ConfigurationEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        [PrimaryKey]
        [Title]
        public string Key { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
