namespace YonderSharp
{
    /// <summary>
    /// Helperclass for parsing stuff
    /// </summary>
    public class ParsingHelper
    {
        /// <summary>
        /// Transforms a string into a double
        /// </summary>
        public static double ParseDouble(string v)
        {
            return double.Parse(v);
        }

        /// <summary>
        /// Transforms the string into a double, with a precession of 5
        /// </summary>
        public static string DoubleToString(double value)
        {
            return value.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
