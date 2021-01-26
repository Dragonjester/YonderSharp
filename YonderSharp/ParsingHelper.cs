namespace YonderSharp
{
    public class ParsingHelper
    {
        public static double ParseDouble(string v)
        {
            return double.Parse(v, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string DoubleToString(double value)
        {
            return value.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
