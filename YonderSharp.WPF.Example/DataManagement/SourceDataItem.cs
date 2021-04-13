using System;
using System.Runtime.Serialization;
using YonderSharp.Attributes;

namespace YonderSharp.WPF.DataManagement.Example
{
    [DataContract]
    public class SourceDataItem
    {
        [PrimaryKey]
        [DataMember]
        public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember]
        public string Title { get; set; }
    }
}
