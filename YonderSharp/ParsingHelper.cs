using System.Globalization;

namespace YonderSharp
{
    /// <summary>
    /// Helperclass for parsing stuff
    /// </summary>
    public class ParsingHelper
    {

        /// <summary>
        /// Transforms the string into a double, with a precession of 5
        /// </summary>
        public static string DoubleToString(double value)
        {
            return value.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Transforms the string into a double, with a precession of 5
        /// Expecting american numberstyle
        /// </summary>
        public static double StringToDouble(string value)
        {
            double step1 = double.Parse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
            string step2 = step1.ToString("0.00000", CultureInfo.InvariantCulture);
            return double.Parse(step2, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"));
        }
    }
}
