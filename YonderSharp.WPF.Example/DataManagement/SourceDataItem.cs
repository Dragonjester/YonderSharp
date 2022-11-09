using System;
using System.Runtime.Serialization;
using YonderSharp.Attributes;
using YonderSharp.WPF.Example.DataManagement;

namespace YonderSharp.WPF.DataManagement.Example
{
    [DataContract]
    public class SourceDataItem
    {
        [PrimaryKey]
        [DataMember]
        public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember]
        [Title]
        public string Title { get; set; }

        [DataMember]
        public SubItem SomeSubItem { get; set; } = new SubItem();
    }
}
