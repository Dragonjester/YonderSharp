using System.Collections.Generic;
using System.Windows.Controls;
using YonderSharp.WPF.Views.StringList;

namespace YonderSharp.WPF.Views
{
    /// <summary>
    /// Interaction logic for StringListView.xaml
    /// </summary>
    public partial class StringListBox : Grid
    {
        public StringListBox()
        {
            IList<string> aList = new List<string>();
            InitializeComponent();
            DataContext = new StringListBoxViewModel(ref aList);
        }

        public StringListBox(ref IList<string> sourceList)
        {
            InitializeComponent();
            DataContext = new StringListBoxViewModel(ref sourceList);
        }

        public StringListBox(StringListBoxViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
