using System.Reflection;

namespace YonderSharp.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <returns>string.empty if the short gid id can't be determined</returns>
        public static string GetShortGidId(this Assembly assembly)
        {
            var errorText = "CANT DETERMINE";

            var fullver = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? $" +{errorText}";
            var gitId = fullver.Split('+')[1];

            if (gitId == errorText)
            {
                return string.Empty;
            }

            return gitId.Substring(0, 7);
        }
    }
}
