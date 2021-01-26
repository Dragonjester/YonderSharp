using System;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.Noise
{
    [DataContract]
    public class NoiseColorRange
    {

        public NoiseColorRange()
        {

        }

        public NoiseColorRange(float minValue, float maxValue, int red, int blue, int green, int alpha, bool transition)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Red = GetCorrectedColorValue(red);
            Blue = GetCorrectedColorValue(blue);
            Green = GetCorrectedColorValue(green);
            Alpha = GetCorrectedColorValue(alpha);
            Transition = transition;
        }

        private int GetCorrectedColorValue(int value)
        {
            return Math.Max(0, Math.Min(255, value));
        }

        [DataMember]
        public float MinValue { get; set; }
        [DataMember]
        public float MaxValue { get; set; }
        [DataMember]
        public int Red { get; set; }
        [DataMember]
        public int Blue { get; set; }
        [DataMember]
        public int Green { get; set; }
        [DataMember]
        public int Alpha { get; set; }
        [DataMember]
        public bool Transition { get; set; }
    }
}
