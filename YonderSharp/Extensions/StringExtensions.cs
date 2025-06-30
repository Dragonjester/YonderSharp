using System;

namespace YonderSharp.Extensions
{
    public static class StringExtensions
    {
        public static bool HasContent(this string str) => !string.IsNullOrEmpty(str);

        public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }
}
