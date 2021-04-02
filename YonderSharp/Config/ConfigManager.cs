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

        public string[] GetAllKeys()
        {
            lock (_locker)
            {
                return _configEntries.Keys.ToArray();
            }
        }

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

        public void SetValue(string key, string value)
        {
            lock (_locker)
            {
                _configEntries[key] = value;
            }
        }

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

        public void Save()
        {
            lock (_locker)
            {
                File.WriteAllText(pathToJson, JsonSerializer.Serialize(_configEntries));
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                _configEntries.Clear();
            }
        }
    }
}
