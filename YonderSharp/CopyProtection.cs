using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace YonderSharp
{
    /// <summary>
    /// The Goal of this isn't to create 100% security, just enough so that stuff can't spread to the wild forever
    /// </summary>
    public class CopyProtection
    {
        private static string[] developerIds = new[] { "0781607453387f55" , "DESKTOP-VBKF9PE", "be6abc1efa0463e7", "8e6e644b116b2354" };
        private static bool hasDeveloperMachineRun;
        private static bool isDeveloperMachine;

        public static bool IsDeveloperMachine()
        {
            if (hasDeveloperMachineRun)
            {
                return isDeveloperMachine;
            }


            var name = System.Net.Dns.GetHostName();

            isDeveloperMachine = developerIds.Contains(name);
            hasDeveloperMachineRun = true;
            return isDeveloperMachine;
        }
        
        public static bool IsDeveloperMachine(IDevice device)
        {
            if (device == null)
            {
                return false;
            }

            if (hasDeveloperMachineRun)
            {
                return isDeveloperMachine;
            }

            var identifier = device.GetUniqueDeviceIdentifier();

            isDeveloperMachine = developerIds.Any(x => string.Equals(x, identifier, StringComparison.OrdinalIgnoreCase));
            hasDeveloperMachineRun = true;

            if (!isDeveloperMachine)
            {
                Debugger.Break();
            }

            return isDeveloperMachine;
        }

    }
}
