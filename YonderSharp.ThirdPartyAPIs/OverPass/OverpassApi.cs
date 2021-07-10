using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;
using KeyValuePair = YonderSharp.ProceduralGeneration.Model.OSM.KeyValuePair;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{
    public class OverpassApi : IOverpassApi
    {
        Random _random = new Random(DateTime.Now.Millisecond * DateTime.Now.Day * DateTime.Now.Year * DateTime.Now.Minute * DateTime.Now.Hour * DateTime.Now.Second);
        private OverpassQueryBuilder _queryBuilder;

        public OverpassApi(OverpassQueryBuilder queryBuilder = null)
        {
            _queryBuilder = queryBuilder ?? new OverpassQueryBuilder();
        }
        private Uri GetOverpassURL()
        {
            //lets not DDOS any server...
            Thread.Sleep(500);

            string[] uris = new[]
            {
                "https://overpass.kumi.systems/api/interpreter",  
          //       "https://overpass-api.de/api/interpreter",
            };

            return new Uri(uris[_random.Next(0, uris.Length - 1)]);
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, Area area)
        {
            string query = _queryBuilder.BuildQueryForNodes(osmLayer, area.YStart, area.YEnd, area.XStart, area.XEnd);
            var nodes = LoadNodesFromOverpass(query);
            return CleanFromToFarAwayEntries(nodes, area.Center, osmLayer.MaxDistanceToCity, osmLayer.MaxDistaToTag).ToArray();
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public IEnumerable<OsmNode> GetOsmNodes(long[] osmIds)
        {
            string query = $"[out:json];node(id:{string.Join(",", osmIds)});out;";

            string resultString = GetOverPassData(query);
            OverPassApiResult result = JsonConvert.DeserializeObject<OverPassApiResult>(resultString);

            foreach (var element in result.Elements)
            {
                if (element.Type.ToLower().Equals("node"))
                {
                    if (element.Tags.TryGetValue("name", out string name))
                    {
                        element.Tags.Remove("name");
                        yield return new OsmNode(element.Id, element.Lat, element.Lon, name, element.Tags.Select(x => new KeyValuePair(x.Key, x.Value)).ToList());
                    }
                }
            }
        }
        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode GetOsmNode(long osmId)
        {
            return GetOsmNodes(new[] { osmId }).First();
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, PointLatLng point, int maxDistance)
        {
            PointLatLng start = WSG84Math.GetDestinationPoint(point, 315, maxDistance);
            PointLatLng end = WSG84Math.GetDestinationPoint(point, 135, maxDistance);

            Area area = new Area();
            area.XStart = start.Longitude;
            area.YStart = start.Latitude;
            area.XEnd = end.Longitude;
            area.YEnd = end.Latitude;

            return GetOsmNodes(osmLayer, area);
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd)
        {
            Area area = new Area();
            area.XStart = longitudeStart;
            area.XEnd = longitudeEnd;
            area.YStart = latitudeStart;
            area.YEnd = latitudeEnd;

            return GetOsmNodes(osmLayer, area);
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode[] GetOsmNodes(OSMPointsLayer layer, double latitude, double longitude, int maxDistance)
        {
            return GetOsmNodes(layer, new PointLatLng(latitude, longitude), maxDistance);
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public OsmNode[] LoadNodesFromOverpass(string csvQuery)
        {
            List<OsmNode> result = new List<OsmNode>();
            string[] data = GetCvsOverPassData(csvQuery);

            if (data == null || data.Length < 2)
            {
                return null;
            }

            //first row determins the Keys of the Tags
            //0 -> id, 1 -> latutide, 2 -> longitude, 3 till whatever -> keys
            string[] firstRow = data[0].Split('\t');

            for (int i = 0; i < data.Length; i++)
            {
                string[] row = data[i].Split('\t');

                for (int y = 0; y < row.Length; y++)
                {
                    row[y] = Cleanup(row[y]);
                }

                OsmNode rowNode = new OsmNode();
                rowNode.ID = long.Parse(row[0]);
                rowNode.Latitude = ParsingHelper.StringToDouble(row[1]);
                rowNode.Longitude = ParsingHelper.StringToDouble(row[2]);
                rowNode.Name = row[3];
                for (int y = 4; y < row.Length; y++)
                {
                    rowNode.Tags.Add(new KeyValuePair(firstRow[y], row[y]));
                }

                result.Add(rowNode);
            }

            //cleanup
            HashSet<long> idHash = new HashSet<long>();
            result = result.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ID != 0 && x.Latitude != 0 && x.Longitude != 0 && idHash.Add(x.ID)).ToList();

            foreach (var entry in result)
            {
                for (int i = 0; i < entry.Tags.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(entry.Tags[i].Value))
                    {
                        entry.Tags.RemoveAt(i);
                        i--;
                    }
                }
            }

            return result.ToArray();
        }

        private string Cleanup(string v)
        {
            return v.Replace("Ã¶", "ö");
        }

        string lastLoadedQuery;
        string lastLoadedResult;

        /// <inheritdoc cref="IOverpassApi"/>
        public string GetOverPassData(string query)
        {
            if (query == lastLoadedQuery && lastLoadedResult != null)
            {
                return lastLoadedResult;
            }

            lastLoadedQuery = query;
            lastLoadedResult = null;

            using (var wb = new WebClient())
            {
                var postBody = query;
                Uri url = GetOverpassURL();
                lastLoadedResult = wb.UploadString(url, postBody);
                return lastLoadedResult;
            }
        }

        private IEnumerable<OsmNode> CleanFromToFarAwayEntries(OsmNode[] nodes, PointLatLng centerPoint, double maxDistanceCities, double maxDistanceTags)
        {
            if (nodes == null || nodes.Length == 0)
            {
                yield break;
            }

            foreach (var node in nodes)
            {
                double distance = WSG84Math.GetDistanceInMeters(centerPoint, node.Latitude, node.Longitude);
                if (distance <= maxDistanceTags || distance <= maxDistanceCities && IsTown(node))
                {
                    yield return node;
                }
            }
        }

        string[] cityKeys = new[] { "town", "village", "suburb", "city" };

        private bool IsTown(OsmNode node)
        {
            if (node == null)
            {
                return false;
            }

            return node.Tags.Any(x => cityKeys.Any(y => y.Equals(x.Key, StringComparison.OrdinalIgnoreCase)));
        }

        /// <inheritdoc cref="IOverpassApi"/>
        public string[] GetCvsOverPassData(string query)
        {
            string content = GetOverPassData(query);
            if (content == null)
            {
                return null;
            }

            var result = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            result.RemoveAt(0); //headerrow

            return result.ToArray();
        }
    }
}