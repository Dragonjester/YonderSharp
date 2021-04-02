using System;
using System.Collections.Generic;
using YonderSharp.Config;
using YonderSharp.WPF.DataManagement;

namespace YonderSharp.WPF.Configuration
{
    public class ConfigurationDataGridSource : IDataGridSource
    {
        private List<ConfigurationEntry> _entries { get; set; } = new List<ConfigurationEntry>();
        public string _searchText { get; set; }
        private IConfigManager _dataSource { get; set; }

        public ConfigurationDataGridSource(IConfigManager dataSource)
        {
            _dataSource = dataSource;
            foreach (string key in dataSource.GetAllKeys())
            {
                _entries.Add(new ConfigurationEntry(key, dataSource.GetValue(key)));
            }
        }

        /// <inheritdoc/>
        public void AddItem(object item)
        {
            _entries.Add((ConfigurationEntry)item);
        }

        /// <inheritdoc/>
        public void AddNewItem()
        {
            _entries.Add(new ConfigurationEntry());
        }

        /// <inheritdoc/>
        public object[] GetAddableItems()
        {
            return null;
        }

        /// <inheritdoc/>
        public object[] GetAllItems()
        {
            return _entries.ToArray();
        }

        /// <inheritdoc/>
        public string GetNameOfIdProperty()
        {
            return "Key";
        }

        /// <inheritdoc/>
        public string GetShownItemTitle(object item)
        {
            return ((ConfigurationEntry)item).Key;
        }

        /// <inheritdoc/>
        public Type GetTypeOfObjects()
        {
            return typeof(ConfigurationEntry);
        }

        /// <inheritdoc/>
        public bool IsFieldPartOfListText(string fieldName)
        {
            return fieldName == "Key";
        }

        /// <inheritdoc/>
        public void RemoveShownItem(object item)
        {
            _entries.Remove((ConfigurationEntry)item);
        }

        /// <inheritdoc/>
        public void Save()
        {
            _dataSource.Clear();
            foreach (ConfigurationEntry entry in _entries)
            {
                _dataSource.SetValue(entry.Key, entry.Value);
            }
            _dataSource.Save();
        }

        /// <inheritdoc/>
        public bool IsAllowedToAddFromList()
        {
            return false;
        }
    }
}
