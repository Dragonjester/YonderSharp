﻿using System;
using System.Collections.Generic;
using YonderSharp.Config;
using YonderSharp.WPF.DataManagement;

namespace YonderSharp.WPF.Configuration
{
    public class ConfigurationDataGridSource : IDataGridSource
    {
        private IConfigManager _dataSource { get; set; }

        public ConfigurationDataGridSource(IConfigManager dataSource)
        {
            _dataSource = dataSource;
            foreach (string key in dataSource.GetAllKeys())
            {
                _items.Add(new ConfigurationEntry(key, dataSource.GetValue(key)));
            }
        }

        /// <inheritdoc/>
        public override Type GetTypeOfObjects()
        {
            return typeof(ConfigurationEntry);
        }

        /// <inheritdoc/>
        public override bool IsFieldPartOfListText(string fieldName)
        {
            return fieldName == "Key";
        }


        private DataGridSourceConfiguration _config;
        protected virtual DataGridSourceConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                _config = new DataGridSourceConfiguration();
                _config.IsAllowedToIsAllowedToAddFromList = false;
                _config.IsAllowedToCreateNewEntry = true;
                _config.IsAllowedToRemove = true;
                _config.HasSearch = true;
                _config.GetAddableItemsReturnAll = true;
            }

            return _config;
        }


        protected override void Save(IList<object> items)
        {
            _dataSource.Clear();
            foreach (ConfigurationEntry entry in items)
            {
                _dataSource.SetValue(entry.Key, entry.Value);
            }
            _dataSource.Save();
        }
    }
}
