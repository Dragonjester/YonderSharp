using System.Collections.Generic;
using System.Windows;

namespace YonderSharp.WPF.Views.DetailList.SelectionDialog
{
    /// <summary>
    /// Interaction logic for SelectionDialogWindow.xaml
    /// </summary>
    public partial class SelectionDialogWindow : Window
    {
        private SelectionDialogDataGridSource source;
        public SelectionDialogWindow(IList<object> knownItems, object selectedItem = null)
        {
            InitializeComponent();

            source = new SelectionDialogDataGridSource(knownItems, SelectItemAction);
            SelectionGrid.SetSource(source, Properties.Resources.Resources.SelectionDialogDataGridSelectButton);

            if(selectedItem != null)
            {
                source.GetConfiguration().ShowSaveButton = false;
                SelectionGrid.SelectItem(selectedItem);
            }
        }

        private void SelectItemAction(object selectedItem)
        {
            DialogResult = selectedItem != null;
            SelectedItem = selectedItem;
        }

        public object SelectedItem { get; private set; }
    }
}
