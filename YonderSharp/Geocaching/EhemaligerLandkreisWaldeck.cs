using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            var textStart = text;

            #region color puzzles
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
                        text = text.Replace($" {anas} ", $" {i} ");
                        text = text.Replace($" {anas}°", $" {i}°");
                        text = text.Replace($" {anas}.", $" {i}.");
                        text = text.Replace($".{anas} ", $".{i} ");
                        
                        if(text.EndsWith($" {anas}"))
                        {
                            text = text.Substring(0, text.Length - $" {anas}".Length) + $" {i}";
                        }
                    }
                }
            }
            #endregion anagramm puzzles

            #region synonym puzzles
            if(text == textStart)
            {
                List<Tuple<string, string>> entries = new List<Tuple<string, string>>();
                entries.Add(new Tuple<string, string>("afgcdeb", "9"));
                entries.Add(new Tuple<string, string>("afedbc", "0"));
                entries.Add(new Tuple<string, string>("afedcb", "0"));
                entries.Add(new Tuple<string, string>("abged", "2"));
                entries.Add(new Tuple<string, string>("afgcd", "5"));
                entries.Add(new Tuple<string, string>("abgcd", "4"));
                entries.Add(new Tuple<string, string>("bc", "1"));

                foreach(var tuple in entries)
                {
                    text = text.Replace($" {tuple.Item1} ", $" {tuple.Item2} ");
                    text = text.Replace($" {tuple.Item1}°", $" {tuple.Item2}°");
                    text = text.Replace($" {tuple.Item1}.", $" {tuple.Item2}.");
                }


                text = text.Replace(" ", "_");
                
                //text = text.Replace("", "2");

                //text = text.Replace("", "3");
                //text = text.Replace("", "4");
               
                //text = text.Replace("", "6");
                //text = text.Replace("", "7");
                //text = text.Replace("", "8");
                //text = text.Replace("", "9");
            }
            #endregion


            #region romanNumbers
            if (text == textStart && !string.IsNullOrEmpty(text))
            {
                Debugger.Break();
            }
            #endregion

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

        private void FillLetters()
        {
            letters = new List<string>[10];

            string[] words = new[] { "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun" };

            for (int i = 0; i < words.Length; i++)
            {
                letters[i] = new List<string>();

                string[] wordLetters = new string[words[i].Length];
                for (int x = 0; x < wordLetters.Length; x++)
                {
                    wordLetters[x] = words[i].ElementAt(x).ToString();
                }


                var permutations = new Permutations();
                //permutations.Generate();
                letters[i] = permutations.GetAllPermutation(wordLetters).ToList();
            }
        }



    }
}
