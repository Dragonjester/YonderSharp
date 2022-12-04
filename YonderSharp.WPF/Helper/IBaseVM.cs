using System.ComponentModel;

namespace YonderSharp.WPF.Helper
{
    public interface IBaseVM : INotifyPropertyChanged
    {
        public CommandMap Commands { get; protected set; }
    }
}
