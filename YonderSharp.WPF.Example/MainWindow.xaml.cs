﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using YonderSharp.Config;
using YonderSharp.WPF.Configuration;
using YonderSharp.WPF.DataManagement.Example;
using YonderSharp.WPF.Helper.CustomDialogs;
using YonderSharp.WPF.Views.ItemSourceComboBox;

namespace YonderSharp.WPF.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IConfigManager _configManager;
        public MainWindow()
        {
            InitializeComponent();

            #region DataGrid
            var source = new SourceDataGridSource("Source");
            var entries = new ExampleDataGridSource("Entry");
            var fileSourceEntries = new DataManagement.ExampleFileSourceDataGridSource();

            SourceGrid.SetSource(source);
            dataGrid.SetSource(entries);
            FileSourceGrid.SetSource(fileSourceEntries);
            #endregion DataGrid

            #region DetaillistBox

            var selectableItems = entries.GetAllItems().ToList();
            ObservableCollection<object> selectedItems = new ObservableCollection<object>();
            for(int i = 0; i < 5; i++)
            {
                selectedItems.Add(selectableItems[i]);
            }

            ADetailListBox.SetSources(selectedItems, selectableItems);


            #endregion DetailListBox

            var icbSource = new ItemSourceForItemsourceCombobox();
            ItemsourceCBox.DataContext = new ItemsourceComboBoxVM(icbSource, ItemsourceCBoxChangeEvent, icbSource.GetAll()[5]);

            _configManager = new ConfigManager();
            _configManager.Load();
        }

        private void ItemsourceCBoxChangeEvent(object obj)
        {
            if(obj == null)
            {
                return;
            }

            if(obj is ExampleDataItem item)
            {
                MessageBox.Show($"Selected item: {item.SomeString}");
            }
        }

        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow configWindow = new ConfigurationWindow(_configManager);
            configWindow.ShowDialog();
        }


        private void ComboBoxDialog(object sender, RoutedEventArgs e)
        {
            ComboboxDialog dialog = new ComboboxDialog(new[] { "1", "2", "3" }, "Title");
            if (dialog.ShowDialog() == true)
            {
                int selectedItem = dialog.SelectedIndex;
            }
        }

        private void InputBoxDialog(object sender, RoutedEventArgs e)
        {
            InputBoxDialog dialog = new InputBoxDialog("Title", "Do you want to build a snowman?");
            dialog.ShowDialog();
            if(dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                string result = dialog.Answer;
            }
        }
    }
}
