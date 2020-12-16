using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;

namespace DeltahedronUI.DataManagement
{
    public class DataGridVM : BaseVM
    {
        private IDataGridSource _dataSource;

        public DataGridVM(IDataGridSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _commands.AddCommand("AddEntry", x => AddEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveEntry());

            UpdateList();
        }

        public Tuple<string, Type>[] GetFields()
        {
            List<Tuple<string, Type>> result = new List<Tuple<string, Type>>();

            Type baseType = _dataSource.GetTypeOfObjects();


            BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;
            var members = baseType.GetProperties(instancePublic)
                .Where(x => Attribute.IsDefined(x, typeof(DataMemberAttribute))
                         && !Attribute.IsDefined(x, typeof(NonSerializedAttribute))).ToList();

            foreach(var member in members)
            {
                string name = member.Name;
                Type type = member.PropertyType;
                //Todo: thing of something to handle non simple datatye classes... which might also be a list, sooooo.... something for version 2  ¯\_(ツ)_/¯
                if (type.IsValueType || type == typeof(string))
                {
                    result.Add(new Tuple<string, Type>(name, type));
                }
            }

            return result.ToArray();
        }

        #region list
        public ObservableCollection<string> ListEntries { get; set; } = new ObservableCollection<string>();
        private void UpdateList()
        {
            ListEntries.Clear();

            foreach (var entry in _dataSource.GetShownItems())
            {
                ListEntries.Add(_dataSource.GetShownItemTitle(entry));
            }

            //TODO: OnPropertyChanged() für die generierten Felder
        }

        #endregion list

        #region selectedItem

        public object SelectedItem
        {
            get
            {
                var items = _dataSource.GetShownItems();
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
            set {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(); 
            }
        }

        #endregion selectedItem

        /// <summary>
        /// Asks the user nicely if he truly wants to remove the selected item.
        /// Depending on how the user answers, the item gets removed from the datasource
        /// </summary>
        private void RemoveEntry()
        {
            if (!_dataSource.IsAllowedToRemove())
            {
                return;
            }

            var selected = SelectedItem;
            if (selected != null)
            {
                var dialog = new ComboboxDialog(new[] { "No", "Yes" }, $"Do you want to remove {_dataSource.GetShownItemTitle(selected)}");
                if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault() && dialog.SelectedIndex == 1)
                {
                    _dataSource.RemoveShownItem(selected);
                }
            }
        }


        private void AddEntry()
        {
            if (!_dataSource.IsAllowedToAddNew())
            {
                return;
            }

            var toExclude = _dataSource.GetShownItems();
            var entries = _dataSource.GetAddableItems().Where(x => !toExclude.Contains(x)).ToArray();

            var dialog = new ComboboxDialog(entries.Select(x => _dataSource.GetShownItemTitle(x)).ToArray(), $"");
            if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault())
            {
                _dataSource.AddShownItem(entries[dialog.SelectedIndex]);
                UpdateList();
            }
        }
    }
}
