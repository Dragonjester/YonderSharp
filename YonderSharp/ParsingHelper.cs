using System;
using System.Collections.Generic;
using System.Globalization;

namespace YonderSharp {

    /// <summary>
    /// Helperclass for parsing stuff
    /// </summary>
    public class ParsingHelper {

        /// <summary>
        /// Transforms the string into a double, with a precession of 5
        /// </summary>
        public static string DoubleToString(double value) {
            return value.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Transforms the string into a double, with a precession of 5
        /// Expecting american numberstyle
        /// </summary>
        public static double StringToDouble(string value) {
            double step1 = double.Parse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
            string step2 = step1.ToString("0.00000", CultureInfo.InvariantCulture);
            return double.Parse(step2, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
        }

        /// <summary>
        /// Fuck Umlaute
        /// </summary>
        public static string ReplaceUmlauteWithXmlEncoding(string value) {
            //yes, there is System.Web.HttpUtility.HtmlEncode(), but this method does too much for the usecase I'm currently having...
            Dictionary<string,string> replacements = new Dictionary<string, string>();
            replacements.Add("ä", "&auml;");
            replacements.Add("ö", "&ouml;");
            replacements.Add("ü", "&uuml;");
            replacements.Add("Ä", "&Auml;");
            replacements.Add("Ö", "&Ouml;");
            replacements.Add("Ü", "&Uuml;");
            replacements.Add("ô", "&ocirc");
            replacements.Add("Ô", "&Ocirc");
            replacements.Add("°", "&deg;");
            replacements.Add("ß","&szlig;");
            replacements.Add("–", "-"); //langes - wird zu kurzem -

            string result = value;

            foreach(var replacementKey in replacements.Keys) {
                result = result.Replace(replacementKey, replacements[replacementKey]);
            }

            return result;

        }
    }
}