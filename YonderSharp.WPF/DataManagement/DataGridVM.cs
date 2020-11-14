using System;
using System.Collections.ObjectModel;
using System.Linq;
using YonderSharp.Modell.DataManagement;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;

namespace DeltahedronUI.DataManagement
{
    public class DataGridVM<T> : BaseVM where T : DataGridItem
    {
        private IDataGridSource<T> _dataSource;

        public DataGridVM(IDataGridSource<T> dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _commands.AddCommand("AddEntry", x => AddEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveEntry());
        }

        #region list
        public ObservableCollection<string> ListEntries { get; set; } = new ObservableCollection<string>();
        private void UpdateList()
        {
            ListEntries.Clear();

            foreach(var entry in _dataSource.GetList())
            {
                ListEntries.Add(entry.GetTitle());
            }

            //TODO: OnPropertyChanged() für die generierten Felder
        }

        #endregion list

        #region selectedItem

        public T SelectedItem
        {
            get
            {
                var items = _dataSource.GetList();
                if (_selectedIndex >= 0 && _selectedIndex < items?.Length)
                {
                    return items[_selectedIndex];
                }

                return null;
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged(); }
        }

        #endregion selectedItem

        private void RemoveEntry()
        {
            var selected = SelectedItem;
            if (selected != null)
            {
                var dialog = new ComboboxDialog(new[] { "No", "Yes" }, $"Do you want to remove {selected.GetTitle()}");
                if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault() && dialog.SelectedIndex == 1)
                {
                    //Step 1: ask if you truly want to remove it.
                    _dataSource.Remove(selected.GetID());
                }
            }
        }


        private void AddEntry()
        {
            var toExclude = _dataSource.GetList();
            var entries = _dataSource.GetPossibleValues().Where(x => !toExclude.Contains(x)).ToArray();

            var dialog = new ComboboxDialog(entries.Select(x => x.GetTitle()).ToArray(), $"");
            if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault())
            {
                //Step 1: ask if you truly want to remove it.
                _dataSource.AddToList(entries[dialog.SelectedIndex].GetID());
                UpdateList();
            }
        }
    }
}
