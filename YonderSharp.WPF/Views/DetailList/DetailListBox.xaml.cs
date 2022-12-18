using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace YonderSharp.WPF.Views.DetailList
{
    /// <summary>
    /// Interaction logic for DetailListBox.xaml
    /// </summary>
    public partial class DetailListBox : Grid
    {
        private DetailListBoxVM _vm;
        public DetailListBox()
        {
            InitializeComponent();
        }

        public void SetSources(ObservableCollection<object> selectedItems, IList<object> selectableItems)
        {
            _vm = new DetailListBoxVM(selectedItems, selectableItems);
            DataContext = _vm;
            _vm.OnSelectionChanged();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(_vm == null && DataContext != null)
            {
                _vm = (DetailListBoxVM)DataContext;
            }

            _vm.ShowSelectedItemDetails();
        }
    }
}