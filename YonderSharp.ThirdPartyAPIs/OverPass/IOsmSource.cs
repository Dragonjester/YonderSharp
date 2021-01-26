using System.Collections.Generic;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{
    public interface IOsmSource
    {
        OsmNode GetOsmNode(long osmId);
        IEnumerable<OsmNode> GetOsmNodes(long[] osmIds);
        OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd);
        OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, Area area);
        OsmNode[] GetOsmNodes(OSMPointsLayer layer, double latitude, double longitude, int maxDistance);
        OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, PointLatLng point, int maxDistance);
    }
}
