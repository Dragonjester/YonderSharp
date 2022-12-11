using System;
using System.Collections.Generic;
using System.Linq;

namespace YonderSharp.WPF.DataManagement
{
    internal class SubItemDataGridSource : IDataGridSource
    {

        private Type itemType { get; set; }

        public SubItemDataGridSource(IList<object> items)
        {
            if(items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            _items.Clear();
            foreach(var item in items)
            {
                _items.Add(item);
            }

            itemType = items.GetType().GenericTypeArguments[0];
        }

        public override object[] GetAddableItems(IList<object> notAddableItems)
        {
            return _items.ToArray();
        }

        public override Type GetTypeOfObjects()
        {
            return itemType;
        }

        /// <inheritdoc/>
        protected override void Save(IList<object> items)
        {
            throw new NotImplementedException();
        }
    }
}