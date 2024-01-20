using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.WPF.Example.DataManagement
{
    [DataContract]
    public class SubItem
    {
        [PrimaryKey]
        [DataMember]
        public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember]
        [Title]
        public string Title { get; set; }
    }
}
