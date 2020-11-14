using System;

namespace YonderSharp.Modell
{
    public class TimeStampedValue<T>
    {
        public DateTime CreatedOnUTC { get; } = DateTime.Now;
        public DateTime LastChange { get; private set; } = DateTime.Now;
        public T Value { get; private set; }
        public void UpdateValue(T newValue)
        {
            Value = newValue;
            LastChange = DateTime.Now;
        }
    }
}
