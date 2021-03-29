using System;
using System.Runtime.Serialization;

namespace YonderSharp.WPF.DataManagement.Example
{
    [DataContract]
    public class SourceDataItem
    {
        [DataMember]
        public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember]
        public string Title { get; set; }
    }
}
