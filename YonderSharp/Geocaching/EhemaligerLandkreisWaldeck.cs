using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace YonderSharp.Geocaching
{
    public class EhemaligerLandkreisWaldeck
    {
        private static List<string>[] letters;

        public EhemaligerLandkreisWaldeck()
        {
            if (letters == null)
            {
                FillLetters();
            }
        }

        public string Solve(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var textStart = text;

            #region roman numbers
            if (text == textStart && (text.Contains("XX")))
            {
                //N LI° XXV. CCCXIV O IX° III.DCLVIII
                var fields = text.Split(new[] { "N", " ", "°", ".", "O", "E" }, StringSplitOptions.RemoveEmptyEntries).Select(x => GetRomanNumber(x)).ToArray();
                if (fields.All(x => x > 0))
                {
                    text = $"N {fields[0]}° {fields[1]}.{fields[2]} E {fields[3]}° {fields[4]}.{fields[5]}";
                }
            }
            #endregion roman numbers


            #region color puzzles
            if (text == textStart)
            {
                text = text.Replace("schwarz", "0");
                text = text.Replace("braun", "1");
                text = text.Replace("rot", "2");
                text = text.Replace("orange", "3");
                text = text.Replace("gelb", "4");
                text = text.Replace("grün", "5");
                text = text.Replace("blau", "6");
                text = text.Replace("violett", "7");
                text = text.Replace("grau", "8");
                text = text.Replace("weiß", "9");
            }
            #endregion color puzzles

            #region keyboard puzzles
            if (text == textStart)
            {
                text = text.Replace("=", "0");
                text = text.Replace("!", "1");
                text = text.Replace("\"", "2");
                text = text.Replace("§", "3");
                text = text.Replace("$", "4");
                text = text.Replace("%", "5");
                text = text.Replace("&", "6");
                text = text.Replace("/", "7");
                text = text.Replace("(", "8");
                text = text.Replace(")", "9");
            }
            #endregion keyboard puzzles

            #region anagramm puzzles
            if (text == textStart)
            {
                for (int i = 0; i < 10; i++)
                {
                    foreach (var anas in letters[i])
                    {
                        text = CoordReplace(text, anas, i);
                    }
                }
            }
            #endregion anagramm puzzles

            #region 7 segment puzzles
            if (text == textStart)
            {
                List<Tuple<string, string>> entries = new List<Tuple<string, string>>();

                entries.Add(new Tuple<string, string>("abcdef", "0"));
                entries.Add(new Tuple<string, string>("bc", "1"));
                entries.Add(new Tuple<string, string>("abdeg", "2"));
                entries.Add(new Tuple<string, string>("abcdg", "3"));
                entries.Add(new Tuple<string, string>("bcfg", "4"));
                entries.Add(new Tuple<string, string>("acdfg", "5"));
                entries.Add(new Tuple<string, string>("afedcg", "6"));
                entries.Add(new Tuple<string, string>("fedcg", "6"));
                entries.Add(new Tuple<string, string>("abc", "7"));
                entries.Add(new Tuple<string, string>("abcdefg", "8"));
                entries.Add(new Tuple<string, string>("afgbc", "9"));


                var permutations = new Permutations();
                List<Tuple<string, string>> temp = new List<Tuple<string, string>>();

                foreach (var tuple in entries)
                {

                    foreach (var permutation in permutations.GetAllStringPermutations(tuple.Item1))
                    {
                        temp.Add(new Tuple<string, string>(permutation, tuple.Item2));
                    }
                }

                entries.AddRange(temp);

                foreach (var tuple in entries)
                {
                    text = CoordReplace(text, tuple.Item1, tuple.Item2);
                }


                if (text != textStart)
                {
                    text = text.Replace(" ", "");
                }
            }
            #endregion 7 segment puzzles

            #region missing number

            if (text == textStart)
            {
                var splitted = text.Split(new[] { ".", "\r", "\n", " ", "°" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length == 16)
                {
                    text = string.Join("", splitted.Select(x => FindMissingNumber(x)));
                    text = text.Substring(0, 3) + "° " + text.Substring(3, 2) + "." + text.Substring(5, 3) + " " + text.Substring(8, 3) + "° " + text.Substring(11, 2) + "." + text.Substring(13);
                }
            }

            #endregion missing number

            text = text.Replace("O", "E");

            text = text.Replace(Environment.NewLine, "");

            text = text.Replace(" ", "");
            text = text.Replace("°", "° ");

            if (!text.Contains(" E"))
            {
                text = text.Replace("E", " E");
            }

            return text;
        }

        Dictionary<string, string> FindMissingNumberCache = new Dictionary<string, string>();
        private string FindMissingNumber(string x)
        {
            if (FindMissingNumberCache.TryGetValue(x, out string result))
            {
                return result;
            }

            string[] whatever = { "N", "E", "O", "", string.Empty };
            if(whatever.Any(wv => string.Equals(wv, x, StringComparison.OrdinalIgnoreCase)))
            {
                FindMissingNumberCache.Add(x, x);
                return x;
            }

            var splitted = x.Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length != 9)
            {
                //wrong format
                Debugger.Break();
                return x;
            }

            for (int i = 0; i < 10; i++)
            {
                if (!splitted.Any(y => i.ToString() == y))
                {
                    FindMissingNumberCache.Add(x, i.ToString());
                    return i.ToString();
                }
            }

            return x;
        }

        private string CoordReplace(string text, string a, string b)
        {
            for (int i = 0; i < 3; i++)
            {
                text = text.Replace($" {a} ", $" {b} ");
            }
            text = text.Replace($" {a}°", $" {b}°");
            text = text.Replace($" {a}.", $" {b}.");
            text = text.Replace($".{a} ", $".{b} ");
            text = text.Replace($" {a}{Environment.NewLine}", $" {b} ");

            if (text.EndsWith($" {a}"))
            {
                text = text.Substring(0, text.Length - $" {a}".Length) + $" {b}";
            }

            return text;
        }

        /// <summary>
        /// Replace {a} with {b} in the context of the whitespaced string
        /// </summary>

        private string CoordReplace(string text, string a, int b)
        {
            return CoordReplace(text, a, b.ToString());
        }

        private int GetRomanNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }


            var number = text.Trim().ToUpper();

            if (!Regex.IsMatch(number, "^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$"))
            {
                return 0;
            }


            var result = 0;

            foreach (var letter in number)
            {
                result += ConvertLetterToNumber(letter);
            }

            if (number.Contains("IV") || number.Contains("IX"))
                result -= 2;

            if (number.Contains("XL") || number.Contains("XC"))
                result -= 20;

            if (number.Contains("CD") || number.Contains("CM"))
                result -= 200;


            return result;



        }

        private int ConvertLetterToNumber(char letter)
        {
            switch (letter)
            {
                case 'M':
                    return 1000;
                case 'D':
                    return 500;
                case 'C':
                    return 100;
                case 'L':
                    return 50;
                case 'X':
                    return 10;
                case 'V':
                    return 5;
                case 'I':
                    return 1;
                default:
                    throw new ArgumentException("Ivalid charakter");
            }
        }

        private void FillLetters()
        {
            letters = new List<string>[10];
            var permutations = new Permutations();
            string[] words = new[] { "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun" };

            for (int i = 0; i < words.Length; i++)
            {
                letters[i] = permutations.GetAllStringPermutations(words[i]).ToList();
            }
        }
    }
}
