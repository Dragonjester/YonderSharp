using System.Runtime.Serialization;

namespace YonderSharp.DataTypes
{
    /// <summary>
    /// Tuple<string, string> but can be used within WPF UI
    /// </summary>
    public class TupleStringString
    {
        [DataMember]
        public string Item1 { get; set; } = string.Empty;

        [DataMember]
        public string Item2 { get; set; } = string.Empty;
    }
}
