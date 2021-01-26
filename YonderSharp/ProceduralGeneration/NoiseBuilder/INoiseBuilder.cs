using System;
using YonderSharp.ProceduralGeneration.Model.Noise;
using YonderSharp.WSG84;

namespace YonderSharp.ProceduralGeneration.NoiseBuilder
{
    public interface INoiseBuilder
    {
        /// <summary>
        /// Gets the Noise value of a layer without any zoom
        /// </summary>
        float GetNoiseValue(NoiseLayer layer, double latitude, double longitude);

        /// <summary>
        /// Gets the Noise value of a layer with zoom
        /// </summary>
        float GetNoiseValue(NoiseConfiguration config, double zoom, double latitude, double longitude);
        float[][] GetNoiseValues(NoiseConfiguration config, double zoom, Area area, double maxDistanceInMeters, int imageWidth, int imageHeight);
        PointLatLng[] GetRandomPoints(RandomPointLayer config, Area area, DateTime dateAndTime);
    }
}
