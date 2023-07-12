using System;

namespace YonderSharp
{
    /// <summary>
    /// The Goal of this isn't to create 100% security, just enough so that stuff can't spread to the wild forever
    /// </summary>
    public class CopyProtection
    {
        public static bool IsDeveloperMachine()
        {
            return Environment.MachineName.Equals("DESKTOP-VBKF9PE");
        }

    }
}
