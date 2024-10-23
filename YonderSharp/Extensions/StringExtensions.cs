namespace YonderSharp.Extensions
{
    public static class StringExtensions
    {
        public static bool HasContent(this string str) => !string.IsNullOrEmpty(str);
    }
}
