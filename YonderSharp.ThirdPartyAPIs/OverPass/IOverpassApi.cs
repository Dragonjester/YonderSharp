using System.Collections.Generic;
using YonderSharp.ProceduralGeneration.Model.OSM;
using YonderSharp.WSG84;

namespace YonderSharp.ThirdPartyAPIs.OverPass
{
    public interface IOverpassApi
    {
        #region general methods
        /// <summary>
        /// Loads data from Overpass
        /// </summary>
        public string GetOverPassData(string query, string cacheFilenamePrefix);

        /// <summary>
        /// Assumes that the query is for CSV values. Returns the result as a row array.
        /// </summary>
        public string[] GetCvsOverPassData(string query, string cacheFilenamePrefix = "");
        #endregion

        #region OsmNode methods
        /// <summary>
        /// returns all nodes found via the query
        /// </summary>
        public OsmNode[] LoadNodesFromOverpass(string csvQuery);

        /// <summary>
        /// Returns all nodes that match the layer within the maxDistance of the point
        /// </summary>
        public OsmNode[] GetOsmNodes(OSMPointsLayer layer, double latitude, double longitude, int maxDistance);

        /// <summary>
        /// Returns all nodes that match the layer within the maxDistance of the point
        /// </summary>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, PointLatLng point, int maxDistance);

        /// <summary>
        /// Returns all nodes that match the layer and are within the given rectangle
        /// </summary>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, double latitudeStart, double latitudeEnd, double longitudeStart, double longitudeEnd);
        /// <summary>
        /// Returns all nodes that match the layer and are within the given rectangle
        /// </summary>
        public OsmNode[] GetOsmNodes(OSMPointsLayer osmLayer, Area area);

        /// <summary>
        /// Returns the node identified by its OSM-ID
        /// </summary>
        public OsmNode GetOsmNode(long osmId);

        /// <summary>
        /// Returns the nodes identified by their OSM-ID
        /// </summary>
        public IEnumerable<OsmNode> GetOsmNodes(long[] osmIds);

        #endregion OsmNode methods
    }
}
