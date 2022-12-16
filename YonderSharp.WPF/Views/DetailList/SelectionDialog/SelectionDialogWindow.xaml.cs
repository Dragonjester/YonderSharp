using System.Collections.Generic;
using System.Windows;

namespace YonderSharp.WPF.Views.DetailList.SelectionDialog
{
    /// <summary>
    /// Interaction logic for SelectionDialogWindow.xaml
    /// </summary>
    public partial class SelectionDialogWindow : Window
    {
        public SelectionDialogWindow(IList<object> knownItems)
        {
            InitializeComponent();
            
            SelectionGrid.SetSource(new SelectionDialogDataGridSource(knownItems, SelectItemAction), Properties.Resources.Resources.SelectionDialogDataGridSelectButton );
        }

        private void SelectItemAction(object selectedItem)
        {
            DialogResult = selectedItem != null;
            SelectedItem = selectedItem;
        }

        public object SelectedItem { get; private set; }

    
    }
}
