using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp.Updater
{
    [DataContract]
    public class FileVersionInfo
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Release { get; set; }
    }
}
