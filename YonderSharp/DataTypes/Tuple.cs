using System.Runtime.Serialization;

namespace YonderSharp.DataTypes
{
    
    /// <summary>
    /// System.Tuple is ReadOnly...
    /// </summary>
    [DataContract]
    public class Tuple<X, Y>
    {
        [DataMember]
        public X Item1 { get; set; }

        [DataMember]
        public Y Item2 { get; set; }
    }
}
