﻿using System;
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

        [ForeignKey(typeof(SourceDataItem), "ID")]
        [DataMember]
        public Guid SourceDataItemRef { get; set; }

        [DataMember]
        public int SomeInt { get; set; }


        [DataMember]
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
    }
}
