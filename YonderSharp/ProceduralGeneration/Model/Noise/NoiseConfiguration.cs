using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace YonderSharp.ProceduralGeneration.Model.Noise
{
    [DataContract]
    public class NoiseConfiguration
    {
        public NoiseConfiguration()
        {
            Colors = new ObservableCollection<NoiseColorRange>();
        }

        public NoiseConfiguration(string title, NoiseType type, int seed, ObservableCollection<NoiseColorRange> colors, float zoom)
        {
            Title = title;
            Type = (int)type;
            Seed = seed;
            Colors = colors ?? new ObservableCollection<NoiseColorRange>();
            Zoom = zoom;

            if (Zoom == 0)
            {
                Zoom = 10000;
            }
        }

        [DataMember]
        public string Title { get; set; }
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

        public string GetIdent()
        {
            return $"{Title} {Type} {Seed} {Zoom} {string.Join(",", Colors.Select(x => x.MinValue + " " + x.MaxValue + " " + x.Alpha + " " + x.Red + " " + x.Green + " " + x.Blue))}";
        }
    }
}
