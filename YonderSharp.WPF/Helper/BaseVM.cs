using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YonderSharp.WPF.Helper
{
    public abstract class BaseVM : INotifyPropertyChanged
    {
        protected CommandMap _commands;

        public BaseVM()
        {
            _commands = new CommandMap();

            //usage:
            //XML: <MenuItem Header="Save" Command="{Binding Commands.SaveCommand}"/>
            //Constructor of VM: _commands.AddCommand("NewExpenseCommand", x => MessageBox.Show("Not yet implemented", "New Expense Claim"));
        }


        /// <summary>
        /// Get the list of commands
        /// </summary>
        public CommandMap Commands
        {
            get { return _commands; }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
