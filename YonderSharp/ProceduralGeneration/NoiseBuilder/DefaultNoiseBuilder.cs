using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using YonderSharp.ProceduralGeneration.Model.Noise;
using YonderSharp.WSG84;

namespace YonderSharp.ProceduralGeneration.NoiseBuilder
{
    public class DefaultNoiseBuilder : INoiseBuilder
    {
        public float GetNoiseValue(NoiseLayer layer, double latitude, double longitude)
        {
            return GetNoiseValue(layer.GetConfiguration(), layer.Zoom, latitude, longitude);
        }

        public float GetNoiseValue(NoiseConfiguration config, double zoom, double latitude, double longitude)
        {
            FastNoise noiseGenerator = new FastNoise(GetSeed(config));
            return GetValue(noiseGenerator, (NoiseType)config.Type, longitude * zoom, latitude * zoom);
        }


        public float[][] GetNoiseValues(NoiseConfiguration config, double zoom, Area area, double maxDistanceInMeters, int imageWidth, int imageHeight)
        {
            if (imageWidth == 0 || imageHeight == 0)
            {
                return null;
            }

            //float factor = Math.Max(imageWidth / 256f, imageHeight / 256f);
            float factor = 1;
            int calcWidth = (int)(imageWidth / factor);
            int calcHeight = (int)(imageHeight / factor);

            float[][] result = new float[calcWidth][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new float[calcHeight];
            }

            FastNoise noiseGenerator = new FastNoise(GetSeed(config));

            double xMovementPerPixel = (area.XEnd - area.XStart) / calcWidth;
            double yMovementPerPixel = (area.YEnd - area.YStart) / calcHeight;

            //Optimisation possibility: we don't have to calculate haversine for each pixel. 
            //Calculate the rectangle that needs to be calculated and set all the other cells to 
            //Color.FromArgb(0, 0, 0, 0)
            //i.e. start in the middle and move to the right and left. Stop calculating when the calculationborder is reached

            bool allPointsIncluded = maxDistanceInMeters == 0 || WSG84Math.GetDistanceInMeters(area.Center, area.TopLeftCorner) < maxDistanceInMeters;

            for (int x = 0; x < calcWidth; x++)
            {
                for (int y = 0; y < calcHeight; y++)
                {
                    double pixelX = area.XStart + xMovementPerPixel * x;
                    double pixelY = area.YStart + yMovementPerPixel * y;

                    result[x][y] = 0;
                    if (allPointsIncluded || WSG84Math.GetDistanceInMeters(pixelY, pixelX, area.YCenter, area.XCenter) < maxDistanceInMeters)
                    {
                        result[x][y] = GetValue(noiseGenerator, (NoiseType)config.Type, pixelX * zoom, pixelY * zoom);
                    }
                }
            }

            return result;
        }

        static Dictionary<int, int> Seeds = new Dictionary<int, int>();
        private int GetSeed(NoiseConfiguration config)
        {
            if (Seeds.TryGetValue(config.Seed, out int value))
            {
                return value;
            }
            else
            {
                Seeds.Add(config.Seed, (int)Math.Sqrt(Math.Abs(13 * config.Seed * config.Seed)));
                return GetSeed(config);
            }
        }

        DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        public PointLatLng[] GetRandomPoints(RandomPointLayer config, Area area, DateTime dateAndTime)
        {
            //Determine fields of the 4 corners
            int north = (int)(area.TopLeftCorner.Latitude / config.FieldSizeWsg84);
            int east = (int)(area.BottomRightCorner.Longitude / config.FieldSizeWsg84);
            int south = (int)(area.BottomRightCorner.Latitude / config.FieldSizeWsg84);
            int west = (int)(area.TopLeftCorner.Longitude / config.FieldSizeWsg84);

            DateTime dateTimeOfPlacing = RoundUp(dateAndTime, new TimeSpan(0, config.TimeIncrementsInMinutes, 0));
            //Get Points of all determined fields
            List<PointLatLng> pointsOfFields = new List<PointLatLng>();
            for (int x = west; x <= east; x++)
            {
                for (int y = south; y < north; y++)
                {
                    Random rand = GetRandomGenerator($"{config.Seed} {dateTimeOfPlacing.ToLongDateString()} {dateTimeOfPlacing.ToLongTimeString()} {x} {y}");
                    double someDouble = rand.NextDouble();
                    if (someDouble <= config.ChanceOfSpawn)
                    {
                        double latitude = y * config.FieldSizeWsg84 + rand.NextDouble() * config.FieldSizeWsg84;
                        double longitude = x * config.FieldSizeWsg84 + rand.NextDouble() * config.FieldSizeWsg84;

                        pointsOfFields.Add(new PointLatLng(latitude, longitude));
                    }
                }
            }

            //only return those points, that are within the area
            return pointsOfFields.Where(x => WSG84Math.IsPointWithinArea(area, x)).ToArray();
        }

        private Random GetRandomGenerator(string seedBase)
        {
            double result = 0;
            foreach (var letter in seedBase.ToCharArray())
            {
                result += Math.Pow(letter, 2);
            }

            return new Random((int)result);
        }

        private Color GetColor(double value, ObservableCollection<NoiseColorRange> colors)
        {
            NoiseColorRange rangeToApply = null;
            foreach (NoiseColorRange range in colors)
            {
                if (range.MinValue <= value && value <= range.MaxValue)
                {
                    rangeToApply = range;
                    break;
                }
            }

            if (rangeToApply == null)
            {
                return Color.FromArgb(0, 0, 0, 0);
            }


            int a = rangeToApply.Alpha;
            int r = rangeToApply.Red;
            int b = rangeToApply.Blue;
            int g = rangeToApply.Green;

            if (rangeToApply.Transition)
            {
                double transitionSize = rangeToApply.MaxValue - rangeToApply.MinValue;
                double transitionProgress = value - rangeToApply.MinValue;

                double factor = Math.Max(0.5, transitionProgress / transitionSize);
                r = (int)(factor * r);
                b = (int)(factor * b);
                g = (int)(factor * g);
            }

            return Color.FromArgb(a, r, g, b);
        }

        /// <returns>0 <= value <= 1</returns>
        private float GetValue(FastNoise noiseGenerator, NoiseType type, double x, double y)
        {
            return GetValue(noiseGenerator, type, (float)x, (float)y);
        }

        /// <returns>0 <= value <= 1</returns>
        private float GetValue(FastNoise noiseGenerator, NoiseType type, float x, float y)
        {
            float val = 0;

            switch (type)
            {
                case NoiseType.Cellular:
                    val = noiseGenerator.GetCellular(x, y);
                    break;
                case NoiseType.Cubic:
                    val = noiseGenerator.GetCubic(x, y);
                    break;
                case NoiseType.CubicFractal:
                    val = noiseGenerator.GetCubicFractal(x, y);
                    break;
                case NoiseType.Perlin:
                    val = noiseGenerator.GetPerlin(x, y);
                    break;
                case NoiseType.PerlinFractal:
                    val = noiseGenerator.GetPerlinFractal(x, y);
                    break;
                case NoiseType.Simplex:
                    val = noiseGenerator.GetSimplex(x, y);
                    break;
                case NoiseType.SimplexFractal:
                    val = noiseGenerator.GetSimplexFractal(x, y);
                    break;
                case NoiseType.Value:
                    val = noiseGenerator.GetValue(x, y);
                    break;
                case NoiseType.ValueFractal:
                    val = noiseGenerator.GetValueFractal(x, y);
                    break;
                case NoiseType.WhiteNoise:
                    val = noiseGenerator.GetWhiteNoise(x, y);
                    break;
            }

            return (val + 1) / 2.0f;
        }
    }
}
