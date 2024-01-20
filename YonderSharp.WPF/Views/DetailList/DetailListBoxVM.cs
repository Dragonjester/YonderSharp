using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using YonderSharp.Attributes.DataManagement;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;
using YonderSharp.WPF.Views.DetailList.SelectionDialog;

namespace YonderSharp.WPF.Views.DetailList
{
    //Sadly WPF doesn't support generics, so wherever you see object, imagine <T>
    public class DetailListBoxVM : BaseVM
    {
        private IList<object> _knownItems { get; set; }
        private ObservableCollection<object> _chosenItems { get; set; }

        /// <param name="knownItems">Items that can be added</param>
        /// <param name="selectedItems">Items that are added</param>
        public DetailListBoxVM(ObservableCollection<object> selectedItems, IList<object> knownItems)
        {
            ValidateLists(selectedItems, knownItems);

            _knownItems = knownItems ?? throw new ArgumentNullException(nameof(knownItems));
            _chosenItems = selectedItems ?? throw new ArgumentNullException(nameof(selectedItems));

            _chosenItems.CollectionChanged += FillShownItems;

            FillShownItems();

            _commands.AddCommand("AddEntry", x => AddEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveSelectedEntry());
       
        }

        private void ValidateLists(ObservableCollection<object> selectedItems, IList<object> knownItems)
        {
            if (selectedItems == null)
            {
                throw new ArgumentNullException(nameof(selectedItems));
            }

            if (knownItems == null)
            {
                throw new ArgumentNullException(nameof(knownItems));
            }

            if (knownItems.Count == 0)
            {
                throw new Exception($"{nameof(knownItems)} is empty");
            }

            if (!Title.TypeHasTitelAttribute(knownItems.First()))
            {
                throw new Exception($"{knownItems.First().GetType().Name} has no Title attribute");
            }

            if(selectedItems.Any(x => !knownItems.Contains(x)))
            {
                throw new Exception("Selected Items contains something that isn't known?!");
            }
        }

        #region Items
        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();

        private void FillShownItems(object? sender = null, NotifyCollectionChangedEventArgs e = null)
        {
            Items.Clear();
            foreach (var item in _chosenItems)
            {
                Items.Add(Title.GetTitel(item));
            }
        }

        #endregion Items

        #region selection
        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnSelectionChanged();
            }
        }

        private bool IsItemSelected()
        {
            return SelectedItem != null;
        }


        public object SelectedItem
        {
            get
            {
                if (SelectedIndex == -1 || SelectedIndex >= _chosenItems.Count)
                {
                    return null;
                }

                return _chosenItems[SelectedIndex];
            }
        }

        public void ShowSelectedItemDetails()
        {
            if (!IsItemSelected())
            {
                return;
            }

            var selectedItem = _chosenItems[SelectedIndex];
            if(selectedItem == null)
            {
                return;
            }

            SelectionDialogWindow dialog = new SelectionDialogWindow(_knownItems, selectedItem);
            var dialogResult = dialog.ShowDialog();
        }

        #endregion selection

        #region Remove Item
        public Visibility ShowRemove
        {
            get => IsItemSelected() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RemoveSelectedEntry()
        {
            if (!IsItemSelected())
            {
                return;
            }

            var selectedIndex = SelectedIndex; //the index can change due to the dialog....

            var dialog = new ComboboxDialog(new[] { "No", "Yes" }, $"Do you want to remove {Title.GetTitel(SelectedItem)}");
            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value && dialog.SelectedIndex == 1)
            {
                _chosenItems.RemoveAt(selectedIndex);
            }
        }
        #endregion Remove Item

        #region Add Item
        private bool _showAdd = true;
        public bool ShowAdd
        {
            get => _showAdd;
            set
            {
                _showAdd = value;
                OnPropertyChanged();
            }
        }

        private void AddEntry()
        {
            SelectionDialogWindow dialog = new SelectionDialogWindow(_knownItems);
            var dialogResult = dialog.ShowDialog();
            if(dialogResult.HasValue && dialogResult.Value && !_chosenItems.Contains(dialog.SelectedItem))
            {
                _chosenItems.Add(dialog.SelectedItem);
                SelectedIndex = _chosenItems.Count - 1;
            }
        }
        #endregion Add Item

    }
}
