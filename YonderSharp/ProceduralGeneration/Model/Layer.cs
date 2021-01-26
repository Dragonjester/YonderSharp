using System;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model
{
    [DataContract]
    public class Layer
    {
        public Layer()
        {

        }


        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public LayerType TypeOfLayer { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
