using System;
using System.Collections.Generic;
using System.Linq;

namespace YonderSharp.WPF.DataManagement.Example
{
    public class SourceDataGridSource : IDataGridSource
    {
        public string _searchText { get; set; }

        public SourceDataGridSource(string prefix = "Item")
        {
            for (int i = 0; i < 10; i++)
            {
                _items.Add(CreateItem(prefix, i));
            }
        }

        private SourceDataItem CreateItem(string prefix, int i)
        {
            SourceDataItem item = new SourceDataItem();
            item.Title = $"{prefix} {i}";
            return item;
        }

        public override Type GetTypeOfObjects()
        {
            return typeof(SourceDataItem);
        }

        private DataGridSourceConfiguration _config;
        public override DataGridSourceConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                _config = new DataGridSourceConfiguration();
                _config.IsAllowedToIsAllowedToAddFromList = false;
                _config.IsAllowedToCreateNewEntry = true;
                _config.IsAllowedToRemove = false;
                _config.HasSearch = false;
                _config.GetAddableItemsReturnAll = true;
                _config.IsReadOnlyMode = true;
            }

            return _config;
        }

        protected override void Save(IList<object> items)
        {
            //Do nothing
        }
    }
}
