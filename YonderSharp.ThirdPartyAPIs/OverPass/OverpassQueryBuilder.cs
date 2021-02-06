using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{
    public class OverpassQueryBuilder
    {

        public string BuildQuery(OSMPointsLayer osmLayer, double latitude, double longitude, double maxDistance = 0)
        {
            if (osmLayer == null)
            {
                return "";
            }
            var nodesToLoad = GetNodesToLoad(osmLayer, maxDistance);

            return BuildQueryForNodes(nodesToLoad.ToArray(), latitude, longitude);
        }

        private IEnumerable<Tuple<string, string, int>> GetNodesToLoad(OSMPointsLayer osmLayer, double maxDistanceInMeters = 0)
        {
            int maxDistanceToCity = (int)maxDistanceInMeters;
            if (osmLayer.MaxDistanceToCity > 0)
            {
                maxDistanceToCity = osmLayer.MaxDistanceToCity;
            }

            int maxDistanceToTag = (int)maxDistanceInMeters;
            if (osmLayer.MaxDistaToTag > 0)
            {
                maxDistanceToTag = osmLayer.MaxDistaToTag;
            }

            if (osmLayer.CityTypeCity)
                yield return new Tuple<string, string, int>("place", "city", maxDistanceToCity);

            if (osmLayer.CityTypeTown)
                yield return new Tuple<string, string, int>("place", "town", maxDistanceToCity);

            if (osmLayer.CityTypeSuburb)
                yield return new Tuple<string, string, int>("place", "suburb", maxDistanceToCity);

            if (osmLayer.CityTypeVillage)
                yield return new Tuple<string, string, int>("place", "village", maxDistanceToCity);

            if (osmLayer.CityTypeNeighbourhood)
                yield return new Tuple<string, string, int>("place", "neighbourhood", maxDistanceToCity);

            foreach (var nodeTag in osmLayer.NodeTags)
                yield return new Tuple<string, string, int>(nodeTag.Key, nodeTag.Value, maxDistanceToTag);
        }

        public string BuildQuery(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd)
        {
            if (osmLayer == null)
            {
                return "";
            }

            double latitude = latitudeStart + (latitudeEnd - latitudeStart) / 2;
            double longitude = longitudeStart + (longitudeEnd - longitudeStart) / 2;
            double maxDistanceInMeters = WSG84Math.GetDistanceInMeters(latitudeStart, longitudeStart, latitudeEnd, longitudeEnd);

            var nodesToLoad = GetNodesToLoad(osmLayer, maxDistanceInMeters);

            return BuildQueryForNodes(nodesToLoad.ToArray(), latitude, longitude);
        }

        /// <summary>
        /// Build Query for nodes
        /// </summary>
        /// <param name="nodesToLoad">key, value, fence (meters)</param>
        public string BuildQueryForNodes(Tuple<string, string, int>[] nodesToLoad, double latitude, double longitude)
        {
            string result = "";

            Dictionary<int, string> calculatedFences = new Dictionary<int, string>();
            PointLatLng centerPoint = new PointLatLng(latitude, longitude);
            foreach (Tuple<string, string, int> tagToLoad in nodesToLoad.Where(x => !string.IsNullOrWhiteSpace(x.Item1)))
            {
                string fence;
                if (!calculatedFences.TryGetValue(tagToLoad.Item3, out fence))
                {
                    PointLatLng botLeft = WSG84Math.GetDestinationPoint(centerPoint, 225, tagToLoad.Item3);
                    PointLatLng topRight = WSG84Math.GetDestinationPoint(centerPoint, 45, tagToLoad.Item3);
                    fence = $"{ParsingHelper.DoubleToString(botLeft.Latitude)},{ParsingHelper.DoubleToString(botLeft.Longitude)},{ParsingHelper.DoubleToString(topRight.Latitude)},{ParsingHelper.DoubleToString(topRight.Longitude)}";
                    calculatedFences.Add(tagToLoad.Item3, fence);
                }

                if (string.IsNullOrEmpty(tagToLoad.Item2) || tagToLoad.Item2 == "*")
                {
                    result += $"node[{tagToLoad.Item1}]({fence});";
                }
                else
                {
                    result += $"node[{tagToLoad.Item1}={tagToLoad.Item2}]({fence});";
                }
            }

            return result;
        }

    }
}
