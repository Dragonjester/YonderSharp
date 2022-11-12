using System;
using System.Collections.Generic;
using System.Linq;

namespace YonderSharp.WPF.DataManagement
{
    internal class SubItemDataGridSource : IDataGridSource
    {
        public string _searchText { get; set; }

        private IList<object> _items { get; set; }
        private Type itemType { get; set; }

        public SubItemDataGridSource(IList<object> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            itemType = items.GetType().GenericTypeArguments[0];
        }

        /// <inheritdoc/>
        public void AddItem(object item)
        {
            _items.Add(item);
        }

        /// <inheritdoc/>
        public void AddNewItem()
        {
            var item = Activator.CreateInstance(GetTypeOfObjects());
            AddItem(item);
        }

        /// <inheritdoc/>
        public object[] GetAddableItems(IList<object> notAddableItems)
        {
            return _items.ToArray();
        }
        /// <inheritdoc/>
        public object[] GetAllItems()
        {
            return _items.ToArray();
        }
        /// <inheritdoc/>
        public Type GetTypeOfObjects()
        {
            return itemType;
        }

        /// <inheritdoc/>
        public void RemoveShownItem(object item)
        {
            _items.Remove(item);
        }
        /// <inheritdoc/>
        public void Save()
        {
            //Saving of SubItems is handeld by the MainItem 
            throw new NotImplementedException();
        }
    }
}