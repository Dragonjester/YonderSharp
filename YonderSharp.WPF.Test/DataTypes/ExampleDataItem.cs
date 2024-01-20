using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.WPF.Test.DataTypes
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
        public string SomeString { get; set; } = string.Empty;


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
