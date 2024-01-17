using System.Windows;
using YonderSharp.Config;

namespace YonderSharp.WPF.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        IConfigManager _manager;
        public ConfigurationWindow(IConfigManager manager)
        {
            InitializeComponent();
            _manager = manager;
            //TODO: CONFIG WIRD NICHT BERÜCKSICHTIGT!!!
            dataGrid.SetSource(new ConfigurationDataGridSource(manager));
        }
    }
}
