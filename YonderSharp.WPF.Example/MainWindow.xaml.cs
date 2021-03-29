using System.Windows;
using YonderSharp.WPF.DataManagement.Example;

namespace YonderSharp.WPF.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dataGrid.SetSource(new ExampleDataGridSource("Entry"));
            SourceGrid.SetSource(new SourceDataGridSource("Source"));
        }
    }
}
