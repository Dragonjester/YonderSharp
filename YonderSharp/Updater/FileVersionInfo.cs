using System.Runtime.Serialization;

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
