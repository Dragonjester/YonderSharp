using System;
using YonderSharp.ThirdPartyAPIs.OverPass.Model;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Pulling
{
    public class ShopPuller : Puller<Shop>
    {
    
        /// <inheritdoc cref="Puller<Shop>"/>
        protected override string[] GetNodeTypes()
        {
            return new[] { "shop", "amenity" };
        }

        /// <inheritdoc cref="Puller<Shop>"/>
        protected override string[] GetPropertiesToLoad()
        {
            return new[] { "name", "shop", "amenity" };
        }

        /// <inheritdoc cref="Puller<Shop>"/>
        protected override Shop GetResultOfRow(string row)
        {
            var entry = row.Split(new[] { '\t' }, StringSplitOptions.None);

            if (entry.Length != 6)
            {
                throw new NotImplementedException($"komische parameterzahl?! DEBUG ME!");
            }

            return new Shop(long.Parse(entry[0]), double.Parse(entry[1].Replace('.', ',')), double.Parse(entry[2].Replace('.', ',')), entry[3], entry[4], entry[5]);
        }        
    }
}
