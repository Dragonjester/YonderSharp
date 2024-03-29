using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using YonderSharp.Attributes.DataManagement;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;
using YonderSharp.WPF.Properties.Resources;

namespace YonderSharp.WPF.DataManagement
{
    public class DataGridVM : BaseVM, IForeignKeyListChangedListener
    {
        public IDataGridSource DataSource { get; private set; }

        private Action _scrollContentIntoNewPositions;
        private Action _scrollToBottomOfList;

        public DataGridVM(IDataGridSource dataSource, Action scrollContentToNewSelection, Action scrollToBottomOfList)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _commands.AddCommand("AddEntryFromList", x => AddEntryFromList());
            _commands.AddCommand("AddNew", x => AddNewEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveEntry());
            _commands.AddCommand("Save", x => Save());

            _scrollContentIntoNewPositions = scrollContentToNewSelection;
            _scrollToBottomOfList = scrollToBottomOfList;

            UpdateList();

            //register ForeignKey Update Listener
            foreach (var foreignType in ForeignKey.GetAllForeignTables(dataSource.GetTypeOfObjects()))
            {
                DataGridSourceManager.RegisterToForeignKeyUpdates(this, foreignType);
            }

            ListEntries.CollectionChanged += this.OnListChanged;
        }

        private void OnListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems?.Count < e.NewItems?.Count)
            {
                _scrollToBottomOfList?.Invoke();
            }
        }

        private void AddNewEntry()
        {
            DataSource.AddNewItem();
            UpdateList();
            SelectedIndex = ListEntries.Count - 1;
            _scrollToBottomOfList?.Invoke();
        }

        public Tuple<string, Type>[] GetFields(Type type)
        {

            List<Tuple<string, Type>> result = new List<Tuple<string, Type>>();


            BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;

            foreach (var member in type.GetProperties(instancePublic))
            {
                string name = member.Name;
                Type memberType = member.PropertyType;

                result.Add(new Tuple<string, Type>(name, memberType));
            }

            return result.ToArray();
        }

        public Tuple<string, Type>[] GetFields()
        {
            //TODO: CONFIG OBJECT, that tells all kind of stuff and can be expanded easily (i.e. readonly)
            //Also: could be configured via XML/JSON/YAML -> and your Grandmother
            return GetFields(DataSource.GetTypeOfObjects());
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
                if (SelectedIndex >= 0 && SelectedIndex < items?.Length)
                {
                    return items[SelectedIndex];
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }

                var items = DataSource.GetShownItems();
                if (items.Contains(value))
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == value)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
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
                _scrollContentIntoNewPositions?.Invoke();
                DataSource.SelectedItem = SelectedItem;
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
            get { return DataSource.GetConfiguration().ShowSaveButton ? Visibility.Visible : Visibility.Collapsed; }
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

        /// <summary>
        /// Label that is used on the save button
        /// </summary>
        private string _saveButtonLabel = Resources.DataGridSaveButton;
        public string SaveButtonLabel
        {
            get => _saveButtonLabel;
            set
            {
                _saveButtonLabel = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadOnlyMode => DataSource.GetConfiguration().IsReadOnlyMode;

        private void Save()
        {
            DataSource.Save();

            //A change might be depending on a new (unsaved) foreign entry
            foreach (var foreignType in ForeignKey.GetAllForeignTables(DataSource.GetType()))
            {
                DataGridSourceManager.GetSource(foreignType)?.Save();
            }

            string messageBoxText = Resources.SavedDialogMessage;
            string caption = Resources.SavedDialogTitle;
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            if (DataSource.GetConfiguration().ShowSaveDialog)
            {
                MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
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
                var dialogResult = dialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value && dialog.SelectedIndex == 1)
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
                if (dialog.ShowDialog() == true)
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

        /// <inheritdoc/>
        public void OnForeignKeyListChanged(Type type)
        {
            OnSelectionChanged();
        }
    }
}
