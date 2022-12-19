using System.Runtime.Serialization;

namespace YonderSharp.DataTypes
{
    /// <summary>
    /// Tuple<string, int> but can be used within WPF UI
    /// </summary>
    [DataContract]
    public class TupleStringInt
    {
        [DataMember]
        public string Item1 { get; set; } = string.Empty;

        [DataMember]
        public int Item2 { get; set; }
    }
}
