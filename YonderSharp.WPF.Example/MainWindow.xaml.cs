using System.Windows;
using YonderSharp.Config;
using YonderSharp.WPF.Configuration;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.DataManagement.Example;

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

            DataGridSourceManager.RegisterDataSource(source);
            DataGridSourceManager.RegisterDataSource(entries);

            SourceGrid.SetSource(source);
            dataGrid.SetSource(entries);
          
            _configManager = new ConfigManager();
            _configManager.Load();
        }

        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {

            ConfigurationWindow configWindow = new ConfigurationWindow(_configManager);
            configWindow.ShowDialog();
        }
    }
}
