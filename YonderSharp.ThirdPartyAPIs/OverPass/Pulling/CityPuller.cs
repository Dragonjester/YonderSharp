using System;
using YonderSharp.ThirdPartyAPIs.OverPass.Model;

namespace YonderSharp.ThirdPartyAPIs.OverPass.Pulling
{
    public class CityPuller : Puller<City>
    {
        /// <inheritdoc cref="Puller<City>"/>
        protected override string[] GetNodeTypes()
        {
            return new[] {
                "place=city",
                "place=town",
                "place=suburb",
                "place=village",
                "place=neighbourhood",
            };
        }

        /// <inheritdoc cref="Puller<City>"/>
        protected override string[] GetPropertiesToLoad()
        {
            return new[] { "name", "place" };
        }

        /// <inheritdoc cref="Puller<City>"/>
        protected override City GetResultOfRow(string row)
        {
            var entry = row.Split(new[] { '\t' }, StringSplitOptions.None);

            if(entry.Length != 4)
            {
                throw new NotImplementedException($"komische parameterzahl?! DEBUG ME!");
            }

           return new City(long.Parse(entry[0]), double.Parse(entry[1].Replace('.', ',')), double.Parse(entry[2].Replace('.', ',')), entry[3], GetCityType(entry[4]));
        }

        private CityType GetCityType(string v)
        {
            switch (v.ToLower().Trim())
            {
                case "city":
                    return CityType.City;
                case "neighbourhood":
                    return CityType.Neighbourhood;
                case "town":
                    return CityType.Town;
                case "village":
                    return CityType.Village;
                case "suburb":
                    return CityType.Suburb;
            }

            throw new NotImplementedException(v);
        }
    }
}