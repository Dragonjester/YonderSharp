using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace YonderSharp.WPF.Helper.CustomDialogs
{
    /// <summary>
    /// Interaction logic for ComboboxDialog.xaml
    /// </summary>
    public partial class ComboboxDialog : Window, INotifyPropertyChanged
    {
        private string[] _items;
        public string[] Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged(); }
        }

        private string title;
        public string WindowTitle { get { return title; } set { title = value; NotifyPropertyChanged(); } }

        private int _selectedIndex;
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string info = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private RelayCommand _okCmd;
        public ICommand OkCmd
        {
            get
            {
                return _okCmd ?? (_okCmd = new RelayCommand(param =>
                {
                    DialogResult = true;
                    Close();
                }));
            }
        }

        private RelayCommand _cancelCmd;
        public ICommand CancelCmd
        {
            get
            {
                return _cancelCmd ?? (_cancelCmd = new RelayCommand(param =>
                {

                    DialogResult = false;
                    Close();
                }));
            }
        }

        public ComboboxDialog(string[] items, string title = "")
        {
            DataContext = this;
            Items = items;
            WindowTitle = title;
            InitializeComponent();
        }

        public bool? ShowDialogInCenterOfCurrent()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            return ShowDialog();
        }
    }
}
