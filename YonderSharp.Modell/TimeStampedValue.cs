using System;
using System.Runtime.Serialization;

namespace YonderSharp.Modell
{
    [DataContract]
    public class TimeStampedValue<T>
    {
        [DataMember]
        public DateTime CreatedOnUTC { get; } = DateTime.Now;
        [DataMember]
        public DateTime LastChange { get; private set; } = DateTime.Now;
        [DataMember]
        public T Value { get; private set; }

        public void UpdateValue(T newValue)
        {
            Value = newValue;
            LastChange = DateTime.Now;
        }
    }
}
