using DeltahedronUI.DataManagement;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace YonderSharp.WPF.DataManagement
{
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class DataGrid : UserControl
    {
        DataGridVM _vm;

        public DataGrid()
        {
            InitializeComponent();
        }

        public DataGrid(IDataGridSource dataSource)
        {
            InitializeComponent();
            SetSource(dataSource);
        }

        private void GenerateFields(Tuple<string, Type>[] items)
        {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                //Add RowDefinition
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                
                //Add Label
                Label label = new Label();
                label.Content = item.Item1 + ":";
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                ContentGrid.Children.Add(label);

                //Add Textbox
                TextBox box = new TextBox();
                Binding bind = new Binding($"SelectedItem.{item.Item1}");
                bind.Source = _vm;
                box.SetBinding(TextBox.TextProperty, bind);
                Grid.SetRow(box, i);
                Grid.SetColumn(box, 1);
                ContentGrid.Children.Add(box);

                //TODO: if(item.Item2 == typeof(bool)) add checkbox
                //TODO: if(string)  textblock?
                
                //TODO: Test if changing data works as expected (Mode? UpdateSource?)
            }
        }

        public void SetSource(IDataGridSource source)
        {
            _vm = new DataGridVM(source);
            DataContext = _vm;

            GenerateFields(_vm.GetFields());
        }


    }
}
