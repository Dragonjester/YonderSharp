using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.Noise
{
    [DataContract]
    public class RandomPointLayer : Layer
    {
        /// <summary>
        /// Max distance to location in meters
        /// </summary>
        public int MaxDistanceInMeters { get; set; }

        /// <summary>
        /// Time increments for new placement, to move the random results around through space AND time
        /// </summary>
        [DataMember]
        public int TimeIncrementsInMinutes { get; set; }

        /// <summary>
        /// 0 < value <= 1
        /// </summary>
        [DataMember]
        public double ChanceOfSpawn { get; set; }

        /// <summary>
        /// Value that tells the size of a RandomPoint Field in WSG84 floating point units.
        /// Positive Numbers only!
        /// </summary>
        [DataMember]
        public double FieldSizeWsg84 { get; set; }

        /// <summary>
        /// seed used for random generator
        /// </summary>
        [DataMember]
        public int Seed { get; set; }
    }
}
