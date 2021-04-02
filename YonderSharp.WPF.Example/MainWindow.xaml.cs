using System.Windows;
using YonderSharp.Config;
using YonderSharp.WPF.Configuration;
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
            dataGrid.SetSource(new ExampleDataGridSource("Entry"));
            SourceGrid.SetSource(new SourceDataGridSource("Source"));

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
