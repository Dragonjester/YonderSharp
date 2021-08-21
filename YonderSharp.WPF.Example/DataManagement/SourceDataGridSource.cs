using System;
using System.Collections.Generic;

namespace YonderSharp.WPF.DataManagement.Example
{
    public class SourceDataGridSource : IDataGridSource
    {
        private List<SourceDataItem> _items = new List<SourceDataItem>();
        private string _prefix;

        public string _searchText { get; set; }

        public SourceDataGridSource(string prefix = "Item")
        {
            _prefix = prefix;
            for (int i = 0; i < 100; i++)
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



        #region add items
        public void AddItem(object item)
        {
            throw new NotImplementedException();
        }

        public void AddNewItem()
        {
            _items.Add(CreateItem(_prefix, _items.Count));
        }

        public object[] GetAddableItems()
        {
            throw new NotImplementedException();
        }
        public bool IsAllowedToAddNew()
        {
            return true;
        }
        #endregion add items
        public object[] GetAllItems()
        {
            return _items.ToArray();
        }

        public Type GetTypeOfObjects()
        {
            return typeof(SourceDataItem);
        }
        /// <inheritdoc/>
        public bool IsFieldPartOfListText(string fieldName)
        {
            return fieldName == "Title";
        }
        /// <inheritdoc/>
        public void RemoveShownItem(object item)
        {
            _items.Remove((SourceDataItem)item);
        }

        public void Save()
        {
        }

        /// <inheritdoc/>
        public virtual bool IsAllowedToCreateNewEntry()
        {
            return true;
        }

        /// <inheritdoc/>
        public virtual bool IsAllowedToAddFromList()
        {
            return false;
        }

        /// <inheritdoc/>
        public bool IsAllowedToRemove()
        {
            return false;
        }
        /// <inheritdoc/>
        public bool HasSearch()
        {
            return false;
        }
        /// <inheritdoc/>
        public string GetNameOfIdProperty()
        {
            return "ID";
        }
    }
}
