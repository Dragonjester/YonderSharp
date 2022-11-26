using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;
using YonderSharp.WPF.Properties.Resources;

namespace YonderSharp.WPF.Views.StringList
{
    public class StringListBoxViewModel : BaseVM
    {
        /// <param name="items">Reference to the string list that will be editable by this view</param>
        public StringListBoxViewModel(ref IList<string> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            foreach (var item in _items)
            {
                Items.Add(item);
            }

            Items.CollectionChanged += SyncSourceList;


            _commands.AddCommand("EditEntry", x => EditSelectedEntry());
            _commands.AddCommand("AddEntry", x => AddEntry());
            _commands.AddCommand("RemoveEntry", x => RemoveSelectedEntry());
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowRemove));
                OnPropertyChanged(nameof(ShowEdit));
            }
        }

        private bool IsItemSelected()
        {
            return SelectedIndex > -1 && SelectedIndex < Items.Count;
        }

        #region Items
        private IList<string> _items;
        private void SyncSourceList(object sender, NotifyCollectionChangedEventArgs e)
        {
            _items.Clear();
            foreach (var item in Items)
            {
                _items.Add(item);
            }
        }

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();

        #endregion Items

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
            InputBoxDialog newStringDialog = new InputBoxDialog(Resources.StringListBoxContextMenuAdd, "");
            newStringDialog.ShowDialog();
            if (!(newStringDialog.DialogResult.HasValue && newStringDialog.DialogResult.Value) || string.IsNullOrWhiteSpace(newStringDialog.Answer))
            {
                return;
            }

            Items.Add(newStringDialog.Answer);
            SelectedIndex = Items.Count - 1;
        }
        #endregion Add Item

        #region Edit Item
        public Visibility ShowEdit
        {
            get => IsItemSelected() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EditSelectedEntry()
        {
            if (!IsItemSelected())
            {
                return;
            }

            InputBoxDialog newStringDialog = new InputBoxDialog(Resources.StringListBoxContextMenuAdd, "", Items[SelectedIndex]);
            newStringDialog.ShowDialog();
            
            if (!(newStringDialog.DialogResult.HasValue && newStringDialog.DialogResult.Value) || string.IsNullOrWhiteSpace(newStringDialog.Answer))
            {
                return;
            }

            Items[SelectedIndex] = newStringDialog.Answer;
        }
        #endregion Edit Item

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

            Items.RemoveAt(SelectedIndex);
        }
        #endregion Remove Item

    }
}
