using System.Collections.Generic;

namespace YonderSharp.Config
{
    public interface IConfigManager
    {
        /// <summary>
        /// Sets all key/value pairs for unknown keys.
        /// If the key is allready known, the existing configuration does not get overwritten!
        /// </summary>
        public void SetDefaultConfig(Dictionary<string, string> defaultConfig);

        public string GetValue(string key);
        public string[] GetAllKeys();
        public void SetValue(string key, string value);
        public void Save();
        public void Load();
        /// <summary>
        /// Deletes all Entries
        /// </summary>
        void Clear();
    }
}
