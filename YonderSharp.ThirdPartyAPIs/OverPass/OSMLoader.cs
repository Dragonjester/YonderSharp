using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;
using KeyValuePair = YonderSharp.ProceduralGeneration.Model.OSM.KeyValuePair;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{
    public class OSMLoader : IOsmSource
    {
        Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Day * DateTime.Now.Year * DateTime.Now.Minute * DateTime.Now.Hour * DateTime.Now.Second);
        private Uri GetOverpassURL()
        {
            string[] uris = new[]
            {
       //         "https://overpass.kumi.systems/api/interpreter",
          //      "http://overpass.openstreetmap.fr/api/interpreter",
                "https://overpass-api.de/api/interpreter",
            };

            return new Uri(uris[random.Next(0, uris.Length - 1)]);
        }


        private string BuildQuery(OSMPointsLayer osmLayer, double latitude, double longitude, double maxDistance = 0)
        {
            if (osmLayer == null)
            {
                return "";
            }
            var nodesToLoad = GetNodesToLoad(osmLayer, maxDistance);

            return BuildQuery(nodesToLoad.ToArray(), latitude, longitude);
        }

        private string BuildQuery(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd)
        {
            if (osmLayer == null)
            {
                return "";
            }

            double latitude = latitudeStart + (latitudeEnd - latitudeStart) / 2;
            double longitude = longitudeStart + (longitudeEnd - longitudeStart) / 2;
            double maxDistanceInMeters = WSG84Math.GetDistanceInMeters(latitudeStart, longitudeStart, latitudeEnd, longitudeEnd);

            var nodesToLoad = GetNodesToLoad(osmLayer, maxDistanceInMeters);

            return BuildQuery(nodesToLoad.ToArray(), latitude, longitude);
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

        private string BuildQuery(Tuple<string, string, int>[] nodesToLoad, double latitude, double longitude)
        {
            var properties = new List<string>();
            properties.AddRange(nodesToLoad?.Select(x => x.Item1).ToList());
            properties = properties.Distinct().ToList();

            if (properties.Count == 0)
            {
                return "";
            }

            string result = $"[out:csv(::id, ::lat, ::lon, name";
            if (properties.Count > 0)
            {
                result += $", { string.Join(",", properties)}";
            }
            result += ")];(";

            if (nodesToLoad?.Count() > 0)
            {
                result += BuildQueryForNodes(nodesToLoad, latitude, longitude);
            }

            return result + "); out;";
        }

        private string BuildQueryForNodes(Tuple<string, string, int>[] nodesToLoad, double latitude, double longitude)
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

        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, Area area)
        {
            string query = BuildQuery(osmLayer, area.YStart, area.YEnd, area.XStart, area.XEnd);
            var nodes = BuildNodesFromQuery(query);
            return CleanFromToFarAwayEntries(nodes, area.Center, osmLayer.MaxDistanceToCity, osmLayer.MaxDistaToTag).ToArray();
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
                if (distance <= maxDistanceTags || distance <= maxDistanceCities && node.Tags.Any(x => x.Key == "place"))
                {
                    yield return node;
                }
            }
        }

        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd)
        {
            Area area = new Area();
            area.XStart = longitudeStart;
            area.XEnd = longitudeEnd;
            area.YStart = latitudeStart;
            area.YEnd = latitudeEnd;

            return GetOsmNodes(osmLayer, area);
        }

        public OsmNode[] GetOsmNodes(OSMPointsLayer layer, double latitude, double longitude, int maxDistance)
        {
            return GetOsmNodes(layer, new PointLatLng(latitude, longitude), maxDistance);
        }

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

        private OsmNode[] BuildNodesFromQuery(string query)
        {
            List<OsmNode> result = new List<OsmNode>();
            string[] data = GetCvsOverPassData(query);

            if (data == null || data.Length < 2)
            {
                return null;
            }

            //first row determins the Keys of the Tags
            //0 -> id, 1 -> latutide, 2 -> longitude, 3 till whatever -> keys
            string[] firstRow = data[0].Split('\t');

            for (int i = 1; i < data.Length; i++)
            {
                string[] row = data[i].Split('\t');

                for (int y = 0; y < row.Length; y++)
                {
                    row[y] = Cleanup(row[y]);
                }

                OsmNode rowNode = new OsmNode();
                rowNode.ID = long.Parse(row[0]);
                rowNode.Latitude = ParsingHelper.ParseDouble(row[1]);
                rowNode.Longitude = ParsingHelper.ParseDouble(row[2]);
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
                try
                {
                    var postBody = query;
                    Uri url = GetOverpassURL();
                    lastLoadedResult = wb.UploadString(url, postBody);
                    return lastLoadedResult;
                }
                catch (Exception e)
                {
                    int trdtrde = 0;
                    throw e;
                }
            }
        }

        public string[] GetCvsOverPassData(string query)
        {
            string content = GetOverPassData(query);
            if (content == null)
            {
                return null;
            }

            return content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public OsmNode GetOsmNode(long osmId)
        {
            return GetOsmNodes(new[] { osmId }).First();
        }

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
    }
}
