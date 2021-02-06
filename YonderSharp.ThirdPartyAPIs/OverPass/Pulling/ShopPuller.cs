using System;
using System.Linq;
using YonderSharp.ThirdPartyAPIs.OverPass.Model;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Pulling
{
    public class ShopPuller : Puller<Shop>
    {
        string[] forbiddenAmenities = new[] {"townhall", "club_house", "fountain", "public_bookcase", "kindergarten", "grave_yard",
"charging_station",
"bicycle_parking",
"ticket",
"fire_station",
"courthouse",
"post_office",
"post_box",
"childcare",
"embassy",
"shelter",
"parking_space",
"motorcycle_parking",
"waste_disposal",
"private_toilet",
"ranger_station",
"nameplate",
"waste_transfer_station",
"crematorium",
"temple",
"weighbridge",
"mortuary",
"emergency_service",
"polling",
"nursery",
"cemetery",
"parking", "prison", "fuel", "baby_hatch", "clinic", "dentist", "doctors", "hospital", "nursing_home", "social_facility", "place_of_worship", "police", "school", "bank", "atm", "bureau_de_change", "ferry_terminal" };



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

            var result = new Shop(long.Parse(entry[0]), double.Parse(entry[1].Replace('.', ',')), double.Parse(entry[2].Replace('.', ',')), entry[3], entry[4], entry[5]);
            if (string.IsNullOrEmpty(result.Name) || forbiddenAmenities.Any(x => string.Equals(x, result.Amenity, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            return result;
        }        
    }
}
