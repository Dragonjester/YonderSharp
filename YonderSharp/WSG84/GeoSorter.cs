using System;
using System.Collections.Generic;
using System.Linq;
using YonderSharp.ProceduralGeneration.Model.OSM;

namespace YonderSharp.WSG84
{
    public class GeoSorter
    {
        public static OsmNode[] MakeKeyValueUniqueToClosest(OsmNode[] nodes, PointLatLng point, string key, string value = null)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            var result = new List<OsmNode>();

            var sortedByDistance = SortByDistance(nodes.Where(x => x.Tags.Any(y => y.Key == key && (y.Value == value || value == null))).ToArray(), point).ToList();
            var keyValueSet = new HashSet<string>();

            for (int i = 0; i < sortedByDistance.Count; i++)
            {
                var keyValuePair = sortedByDistance[i].Tags.First(x => x.Key == key && (x.Value == value || value == null));
                if (!keyValueSet.Add($"{keyValuePair.Key} {keyValuePair.Value}".ToLower()))
                {
                    sortedByDistance.Remove(sortedByDistance[i]);
                    i--;
                }
            }

            foreach (var node in nodes)
            {
                var keyValue = node.Tags.FirstOrDefault(x => x.Key == key);
                if (keyValue == null || sortedByDistance.Contains(node))
                {
                    result.Add(node);
                }
            }

            return result.ToArray();
        }

        public static OsmNode[] BringTagToStart(OsmNode[] nodes, string key)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(nodes));
            }


            List<OsmNode> result = new List<OsmNode>();
            foreach (var node in nodes)
            {
                if (node.Tags.Any(x => x.Key == key))
                {
                    result.Insert(0, node);
                }
                else
                {
                    result.Add(node);
                }
            }

            return result.ToArray();
        }

        public static OsmNode[] SortByDistance(OsmNode[] nodes, PointLatLng point, bool ascending = true)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (ascending)
            {
                return nodes.OrderBy(x => WSG84Math.GetDistanceInMeters(new PointLatLng(x.Latitude, x.Longitude), point)).ToArray();
            }
            else
            {
                return nodes.OrderByDescending(x => WSG84Math.GetDistanceInMeters(new PointLatLng(x.Latitude, x.Longitude), point)).ToArray();
            }
        }
    }
}
