using System;
using YonderSharp.Extensions;

namespace YonderSharp
{
    public class LevenshteinDistanceCalculator
    {
        /// <summary>
        /// Calculates the Levenshtein distance between two strings
        /// </summary>
        /// <param name="first">The first string</param>
        /// <param name="second">The second string</param>
        /// <returns>The number of changes that need to be made to convert the first string to the second.</returns>
        public int CalculateLevenshteinDistance(string first, string second)
        {
            if (!first.HasContent() && !second.HasContent())
            {
                return 0;
            }

            if (!first.HasContent())
            {
                return second.Length;
            }

            if (!second.HasContent())
            {
                return first.Length;
            }          

            if(string.Equals(first, second, StringComparison.InvariantCulture))
            {
                return 0;
            }

            int n = first.Length;
            int m = second.Length;
            var d = new int[n + 1, m + 1]; // matrix

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {

                for (int j = 1; j <= m; j++)
                {
                    int cost = (second.Substring(j - 1, 1) == first.Substring(i - 1, 1) ? 0 : 1); // cost
                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1,
                            d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }



        /// <summary>
        /// Calculates the Levenshtein distance between two strings, without being case sensitive
        /// </summary>
        /// <param name="first">The first string</param>
        /// <param name="second">The second string</param>
        /// <returns>The number of changes that need to be made to convert the first string to the second.</returns>
        public int CalculateLevenshteinDistanceIgnoringCase(string first, string second)
        {
            if (!first.HasContent() && !second.HasContent())
            {
                return 0;
            }

            if (!first.HasContent())
            {
                return second.Length;
            }

            if (!second.HasContent())
            {
                return first.Length;
            }
            return CalculateLevenshteinDistance(first.ToLower(), second.ToLower());
        }
    }
}
