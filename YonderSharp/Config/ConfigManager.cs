using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace YonderSharp.Config
{
    public class ConfigManager : IConfigManager
    {
        Dictionary<string, string> _configEntries = new Dictionary<string, string>();
        object _locker = new object();
        string pathToJson;

        public ConfigManager()
        {
            pathToJson = AppDomain.CurrentDomain.BaseDirectory + "\\configmanager.json";
        }

        /// <inheritdoc/>
        public string[] GetAllKeys()
        {
            lock (_locker)
            {
                return _configEntries.Keys.ToArray();
            }
        }

        /// <inheritdoc/>
        public string GetValue(string key)
        {
            lock (_locker)
            {
                try
                {
                    return _configEntries[key];
                }
                catch (KeyNotFoundException knfe)
                {
                    return "";
                }
            }
        }

        /// <inheritdoc/>
        public void SetValue(string key, string value)
        {
            lock (_locker)
            {
                _configEntries[key] = value;
            }
        }

        /// <inheritdoc/>
        public void Load()
        {
            lock (_locker)
            {
                if (File.Exists(pathToJson))
                {
                    try
                    {
                        _configEntries = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(pathToJson));
                    }
                    catch (Exception e)
                    {
                        //TODO: LOGGING
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Save()
        {
            lock (_locker)
            {
                File.WriteAllText(pathToJson, JsonSerializer.Serialize(_configEntries));
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (_locker)
            {
                _configEntries.Clear();
            }
        }

        /// <inheritdoc/>
        public void SetDefaultConfig(Dictionary<string, string> defaultConfig)
        {
            foreach(string key in defaultConfig.Keys.Where(x => !_configEntries.ContainsKey(x)))
            {
                _configEntries.Add(key, defaultConfig[key]);
            }
        }
    }
}