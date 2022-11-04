using System.Windows;
using YonderSharp.Config;
using YonderSharp.WPF.Configuration;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.DataManagement.Example;
using YonderSharp.WPF.Helper.CustomDialogs;

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

            var source = new SourceDataGridSource("Source");
            var entries = new ExampleDataGridSource("Entry");
            var fileSourceEntries = new DataManagement.ExampleFileSourceDataGridSource();

            DataGridSourceManager.RegisterDataSource(source);
            DataGridSourceManager.RegisterDataSource(entries);
            DataGridSourceManager.RegisterDataSource(fileSourceEntries);

            SourceGrid.SetSource(source);
            dataGrid.SetSource(entries);
            FileSourceGrid.SetSource(fileSourceEntries);

            _configManager = new ConfigManager();
            _configManager.Load();
        }

        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow configWindow = new ConfigurationWindow(_configManager);
            configWindow.ShowDialog();
        }


        private void ComboBoxDialog(object sender, RoutedEventArgs e)
        {
            ComboboxDialog dialog = new ComboboxDialog(new[] { "1", "2", "3" }, "Title");
            if (dialog.ShowDialogInCenterOfCurrent())
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
