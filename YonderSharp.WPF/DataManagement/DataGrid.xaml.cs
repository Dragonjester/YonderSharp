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
            EntryList.SelectionChanged += (s, e) => EntryList.ScrollIntoView(EntryList.SelectedItem);
        }

        private void GenerateFields(Tuple<string, Type>[] items)
        {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
              
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //Add Label
                Label label = new Label();
                label.Content = _vm.DataSource.GetLabel(item.Item1);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                ContentGrid.Children.Add(label);

                UIElement currentElement;
                //Add content element
                if (item.Item2 == typeof(bool))
                {
                    CheckBox box = new CheckBox();
                    box.VerticalAlignment = VerticalAlignment.Center;
                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
                    bind.Source = _vm;
                    box.SetBinding(CheckBox.IsCheckedProperty, bind);
                    Grid.SetRow(box, i);
                    Grid.SetColumn(box, 1);
                    ContentGrid.Children.Add(box);

                    currentElement = box;
                }
                else
                {
                    TextBox box = new TextBox();
                    box.VerticalContentAlignment = VerticalAlignment.Center;
                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
                    bind.Source = _vm;
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    box.SetBinding(TextBox.TextProperty, bind);
                    Grid.SetRow(box, i);
                    Grid.SetColumn(box, 1);
                    ContentGrid.Children.Add(box);

                    box.Margin = new Thickness(0, 2, 0, 2);

                    currentElement = box;
                }

                //TODO: Dropdown for Enums

                if(currentElement == null)
                {
                    throw new Exception("You forgot to set the current element Ü");
                }

                

                if (_vm.DataSource.IsFieldPartOfListText(item.Item1))
                {
                    currentElement.LostFocus += RefreshList;
                }
            }
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            _vm.UpdateList();
        }

        public void SetSource(IDataGridSource source)
        {
            _vm = new DataGridVM(source);
            DataContext = _vm;

            GenerateFields(_vm.GetFields());
        }


    }
}
