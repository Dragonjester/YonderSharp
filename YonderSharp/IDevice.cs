using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp
{
    public interface IDevice
    {
        public string GetUniqueDeviceIdentifier();

        public bool IsWindowsMachine();

        public Task<string> GetFile(string[] allowedFileEndings = null);
    }
}
