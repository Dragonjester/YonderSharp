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

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return Environment.MachineName.Equals("DESKTOP-VBKF9PE");
                default:
                    //TODO: check for android phone/tablet
                    //Android, Linux, Mac, etc.
                    return true;
            }


        }

    }
}
