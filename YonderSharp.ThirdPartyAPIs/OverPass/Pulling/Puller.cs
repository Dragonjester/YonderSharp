using System.Collections.Generic;
using System.Threading;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Pulling
{
    public abstract class Puller<T>
    {
        /// <param name="api">can be null</param>
        public Puller(IOverpassApi api)
        {
            _api = api ?? new OverpassApi();
        }

        /// <summary>
        /// 
        /// </summary>
        public Puller() : this(new OverpassApi())
        {

        }

        protected int LatitudeStepSize = 1;
        protected int LongitudeStepSize = 1;
        private IOverpassApi _api;

        /// <summary>
        /// returns the properties that are to be loaded from the nodes
        /// </summary>
        /// <returns></returns>
        protected abstract string[] GetPropertiesToLoad();

        /// <summary>
        /// returns the node types that are to be loaded
        /// </summary>
        protected abstract string[] GetNodeTypes();

        /// <summary>
        /// Pulls the required data from the overpass api for the given rectangle
        /// </summary>
        public virtual IEnumerable<T> Pull(int latitudeStart, int longitudeStart, int latitudeEnd, int longitudeEnd, int sleepBetweenPullsInMs = 5000)
        {
            //ensure that start > end...
            if (latitudeEnd < latitudeStart)
            {
                var temp = latitudeStart;
                latitudeStart = latitudeEnd;
                latitudeEnd = temp;
            }

            if (longitudeEnd < longitudeStart)
            {
                var temp = longitudeStart;
                longitudeStart = longitudeEnd;
                longitudeEnd = temp;
            }


            for (int latitude = latitudeStart; latitude <= latitudeEnd; latitude += LatitudeStepSize)
            {
                for (int longitude = longitudeStart; longitude <= longitudeEnd; longitude += LongitudeStepSize)
                {
                    foreach (string row in _api.GetCvsOverPassData(GetOverPassPostBody(latitude, longitude)))
                    {
                        var result = GetResultOfRow(row);
                        if (result != null)
                        {
                            yield return result;
                        }
                    }

                    if (sleepBetweenPullsInMs > 0)
                    {
                        Thread.Sleep(sleepBetweenPullsInMs);
                    }
                }
            }
        }

        protected string GetOverPassPostBody(double latitude, double longitude)
        {
            string tags = string.Join(",", GetPropertiesToLoad());

            string result = $"[out:csv(::id, ::lat, ::lon, {tags})];(";

            foreach (string nodeType in GetNodeTypes())
            {
                result += $"node[{nodeType}]({latitude},{longitude},{latitude + 1},{longitude + 1});";
            }

            result += "); out;";

            return result;
        }

        /// <summary>
        /// transforms the csv row to your object
        /// </summary>
        protected abstract T GetResultOfRow(string row);
    }
}