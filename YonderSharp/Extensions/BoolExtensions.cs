namespace YonderSharp.Extensions
{
    public static class BoolExtensions
    {
        public static bool IsTrue(this bool? value)
        {
            if(value == null)
            {
                return false;
            }

            return value.Value;
        }
    }
}
