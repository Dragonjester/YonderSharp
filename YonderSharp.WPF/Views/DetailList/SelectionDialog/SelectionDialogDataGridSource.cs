using System;
using System.Collections.Generic;
using System.Linq;
using YonderSharp.WPF.DataManagement;

namespace YonderSharp.WPF.Views.DetailList.SelectionDialog
{
    internal class SelectionDialogDataGridSource : IDataGridSource
    {
        private Action<object> _selectedItemAction;

        public SelectionDialogDataGridSource(IList<object> knownItems, Action<object> selectItemAction) {
            if(knownItems == null || knownItems.Count == 0)
            {
                throw new ArgumentNullException(nameof(knownItems));
            }

            _selectedItemAction = selectItemAction ?? throw new ArgumentNullException(nameof(selectItemAction));

            foreach (var item in knownItems)
            {
                _items.Add(item);
            }
        }
        public override Type GetTypeOfObjects()
        {
            return _items.First().GetType();
        }

        protected override void Save(IList<object> items)
        {
            _selectedItemAction.Invoke(SelectedItem);
        }

        public override DataGridSourceConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                _config = new DataGridSourceConfiguration();
                _config.IsAllowedToIsAllowedToAddFromList = false;
                _config.IsAllowedToCreateNewEntry = false;
                _config.IsAllowedToRemove = false;
                _config.HasSearch = true;
                _config.GetAddableItemsReturnAll = true;
                _config.IsPrimaryKeyDisabled = true;
                _config.ShowSaveDialog = false;
                _config.IsReadOnlyMode = true;
            }

            return _config;
        }
    }
}
