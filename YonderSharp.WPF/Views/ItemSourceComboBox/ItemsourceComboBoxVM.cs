using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using YonderSharp.FileSources;
using YonderSharp.WPF.Helper;

namespace YonderSharp.WPF.Views.ItemSourceComboBox
{
    public class ItemsourceComboBoxVM : BaseVM
    {
        private ItemSourceForLists _source;
        private Action<object> _onSelectedObjectChanged;
        private int _selectedIndex = -1;
        private MethodInfo ElementAt;

        public ItemsourceComboBoxVM(ItemSourceForLists source, Action<object> selectedItemChanged, object selectedItem = null)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));

            if (!source.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ItemSource<>)))
            {
                throw new ArgumentException($"the source needs to be ItemSource");
            }

            ItemSource = source.GetTitles();

            if (selectedItem != null)
            {
                SelectedIndex = _source.GetIndexOf(selectedItem);
            }

            _onSelectedObjectChanged = selectedItemChanged;

            ElementAt = _source.GetType().GetMethods().First(x => x.Name == "ElementAt");

            OnSelectionChanged();
        }


        public ObservableCollection<string> ItemSource { get; set; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value)
                {
                    return;
                }

                _selectedIndex = value;

                OnPropertyChanged();
                try
                {
                    _onSelectedObjectChanged?.Invoke(ElementAt.Invoke(_source, new object[] { value }));
                }
                catch (Exception e)
                {
                    Debugger.Break();
                }
            }
        }
    }
}