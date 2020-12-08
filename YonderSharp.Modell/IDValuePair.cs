using System;
using System.Runtime.Serialization;

namespace YonderSharp.Modell
{
    [DataContract]
    public class IDValuePair<T>
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public T Value { get; set; }
    }
}
