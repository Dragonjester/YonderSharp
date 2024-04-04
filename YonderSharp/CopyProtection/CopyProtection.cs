using System;
using System.Diagnostics;
using System.Linq;

namespace YonderSharp.CopyProtection
{
    /// <summary>
    /// The Goal of this isn't to create 100% security. The goal is to prohibit that {stuff} can spread to the wild uncontrolled forever
    /// </summary>
    public partial class CopyProtection
    {
        private static string[] developerIds = new[] { "0781607453387f55", "DESKTOP-VBKF9PE", "be6abc1efa0463e7", "8e6e644b116b2354" };
        private static bool hasDeveloperMachineRun;
        private static bool isDeveloperMachine;


        /// <summary>
        /// Returns the date of the last day an application is allowed to run
        /// </summary>
        public static DateTime LastAllowedDay()
        {
            return GetCompileDate().AddDays(365);
        }

        public static bool IsAllowedToRun()
        {
            return IsDeveloperMachine() || DateTime.Now <= LastAllowedDay();
        }

        public static bool IsAllowedToRun(IDevice device)
        {
            return IsDeveloperMachine(device) || DateTime.Now <= LastAllowedDay();
        }

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
            if (hasDeveloperMachineRun)
            {
                return isDeveloperMachine;
            }

            if (device == null)
            {
                return false;
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