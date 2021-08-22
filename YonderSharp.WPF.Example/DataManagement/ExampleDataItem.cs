using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using YonderSharp.Attributes;

namespace YonderSharp.WPF.DataManagement.Example
{
    [DataContract]
    public class ExampleDataItem
    {
        [PrimaryKey]
        [DataMember]
        public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember]
        public int SomeInt { get; set; }


        [DataMember]
        [Title]
        public string SomeString { get; set; }


        [DataMember]
        public long SomeLong { get; set; }


        [DataMember]
        public bool SomeBool { get; set; }


        [DataMember]
        public float SomeFloat { get; set; }

        [DataMember]
        public decimal SomeDecimal { get; set; }

        [DataMember]
        public double SomeDouble { get; set; }

        [ForeignKey(typeof(SourceDataItem), "ID")]
        [DataMember]
        public Guid SourceDataItemRef { get; set; } = Guid.Empty;

        [ForeignKey(typeof(SourceDataItem), "ID")]
        [DataMember]
        public List<Guid> SourceDataItemRefs { get; set; } = new List<Guid>();
    }
}
