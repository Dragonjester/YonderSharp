using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using YonderSharp.Attributes;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;

namespace DeltahedronUI.DataManagement
{
    public class DataGridVM : BaseVM
    {
        public IDataGridSource DataSource { get; private set; }

        public DataGridVM(IDataGridSource dataSource)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _commands.AddCommand("AddEntryFromList", x => AddEntryFromList());
            _commands.AddCommand("AddNew", x => AddNewEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveEntry());
            _commands.AddCommand("Save", x => Save());

            UpdateList();
        }

        private void AddNewEntry()
        {
            DataSource.AddNewItem();
            UpdateList();
            SelectedIndex = ListEntries.Count - 1;
        }

        public Tuple<string, Type>[] GetFields()
        {
            //TODO: CONFIG OBJECT, that tells all kind of stuff and can be expanded easily (i.e. readonly)
            //Also: could be configured via XML/JSON/YAML -> and your Grandmother

            List<Tuple<string, Type>> result = new List<Tuple<string, Type>>();

            Type baseType = DataSource.GetTypeOfObjects();

            BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;

            foreach (var member in baseType.GetProperties(instancePublic))
            {
                string name = member.Name;
                Type type = member.PropertyType;

                result.Add(new Tuple<string, Type>(name, type));
            }

            return result.ToArray();
        }

        public virtual IEnumerable GetEnumNames(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Not an enum?!");
            }

            return Enum.GetNames(enumType);
        }

        #region list
        public ObservableCollection<string> ListEntries { get; set; } = new ObservableCollection<string>();
        public void UpdateList()
        {
            int index = SelectedIndex;
            ListEntries.Clear();

            foreach (var entry in DataSource.GetShownItems())
            {
                ListEntries.Add(DataSource.GetShownItemTitle(entry));
            }

            SelectedIndex = index;
            if (SelectedIndex >= ListEntries.Count)
            {
                SelectedIndex = 0;
            }
        }

        #endregion list

        #region selectedItem

        public object SelectedItem
        {
            get
            {
                var items = DataSource.GetShownItems();
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
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        #endregion selectedItem

        public Visibility ShowRemove
        {
            get { return DataSource.IsAllowedToRemove() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ShowOnlyAddNew
        {
            get { return DataSource.IsAllowedToAddNew() && !DataSource.IsAllowedToAddFromList() ? Visibility.Visible : Visibility.Collapsed; }
        }


        /// <summary>
        /// Should the textbox for the primary field key be disabled?
        /// </summary>
        public bool IsPrimaryKeyDisabled
        {
            get { return DataSource.IsPrimaryKeyDisabled(); }
        }

        public Visibility ShowOnlyAddFromList
        {
            get { return DataSource.IsAllowedToAddFromList() && !DataSource.IsAllowedToAddNew() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ShowMergedAdd
        {
            get { return DataSource.IsAllowedToAddFromList() && DataSource.IsAllowedToAddNew() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CanSave
        {
            get { return DataSource.HasSearch() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CanSearch
        {
            get { return DataSource.HasSearch() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string SearchText
        {
            get
            {
                return DataSource.GetSearchText() ?? "";
            }
            set
            {
                DataSource.SetSearchText(value);
                OnPropertyChanged();
                UpdateList();
            }
        }

        private void Save()
        {
            if (DataSource.HasSearch())
            {
                DataSource.Save();
            }
        }

        /// <summary>
        /// Asks the user nicely if he truly wants to remove the selected item.
        /// Depending on how the user answers, the item gets removed from the datasource
        /// </summary>
        private void RemoveEntry()
        {
            if (!DataSource.IsAllowedToRemove())
            {
                return;
            }

            var selected = SelectedItem;
            if (selected != null)
            {
                var dialog = new ComboboxDialog(new[] { "No", "Yes" }, $"Do you want to remove {DataSource.GetShownItemTitle(selected)}");
                if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault() && dialog.SelectedIndex == 1)
                {
                    DataSource.RemoveShownItem(selected);
                    UpdateList();
                }
            }
        }

        private void AddEntryFromList()
        {
            if (!DataSource.IsAllowedToAddNew())
            {
                return;
            }

            if (DataSource.IsAllowedToAddFromList())
            {
                var toExclude = DataSource.GetShownItems();
                var entries = DataSource.GetAddableItems(toExclude);

                var dialog = new ComboboxDialog(entries.Select(x => DataSource.GetShownItemTitle(x)).ToArray(), $"");
                if (dialog.ShowDialogInCenterOfCurrent().GetValueOrDefault())
                {
                    DataSource.AddItem(entries[dialog.SelectedIndex]);
                    string title = DataSource.GetShownItemTitle(entries[dialog.SelectedIndex]);
                    UpdateList();

                    for (int i = 0; i < ListEntries.Count; i++)
                    {
                        if (ListEntries[i] == title)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
    }
}
