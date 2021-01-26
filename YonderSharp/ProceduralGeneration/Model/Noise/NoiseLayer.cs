using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.Noise
{
    [DataContract]
    public class NoiseLayer : Layer
    {
        public NoiseLayer()
        {
            Colors = new ObservableCollection<NoiseColorRange>();
            TypeOfLayer = LayerType.Noise;
        }

        public NoiseLayer(string title, NoiseType type, int seed, ObservableCollection<NoiseColorRange> colors, float zoom, int maxDistance)
        {
            Title = title;
            TypeOfLayer = LayerType.Noise;
            ID = Guid.NewGuid();
            Type = (int)type;
            Seed = seed;
            Colors = colors ?? new ObservableCollection<NoiseColorRange>();
            Zoom = zoom;

            if (Zoom == 0)
            {
                Zoom = 10000;
            }

            MaxDistance = maxDistance;
        }

        /// <summary>
        /// -> NoiseType enum
        /// </summary>
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public int Seed { get; set; }
        [DataMember]
        public ObservableCollection<NoiseColorRange> Colors { get; set; }
        [DataMember]
        public float Zoom { get; set; }
        [DataMember]
        public int MaxDistance { get; set; }

        public string GetIdent()
        {
            return $"{Title} {Type} {Seed} {Zoom} {MaxDistance} {string.Join(",", Colors.Select(x => x.MinValue + " " + x.MaxValue + " " + x.Alpha + " " + x.Red + " " + x.Green + " " + x.Blue))}";
        }

        /// <returns>Configuration representation of the Layer</returns>
        public NoiseConfiguration GetConfiguration()
        {
            return new NoiseConfiguration(Title, (NoiseType)Type, Seed, Colors, Zoom);
        }
    }
}
