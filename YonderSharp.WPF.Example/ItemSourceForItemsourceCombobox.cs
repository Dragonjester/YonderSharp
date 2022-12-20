using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using YonderSharp.FileSources;
using YonderSharp.WPF.DataManagement.Example;

namespace YonderSharp.WPF.Example
{
    public class ItemSourceForItemsourceCombobox : ItemSource<ExampleDataItem>
    {
        private IList<ExampleDataItem> items;

        public ItemSourceForItemsourceCombobox()
        {
            items = new List<ExampleDataItem>();
            for (int i = 0; i < 20; i++)
            {
                Add(new ExampleDataItem() { SomeString = $"{1 + i}" });
            }
        }

        public void Add(ExampleDataItem obj)
        {
            items.Add(obj);
            RefillTitles();
        }

        public void Add(IList<ExampleDataItem> list)
        {
            if (list == null)
            {
                return;
            }

            foreach (var item in list)
            {
                items.Add(item);
            }
        }

        public void Add(IList<object> list)
        {
            if (list == null)
            {
                return;
            }

            foreach (var item in list)
            {
                if (item is ExampleDataItem casted)
                {
                    Add(casted);
                }
            }
        }

        public void Add(object item)
        {
            if (item is ExampleDataItem casted)
            {
                Add(casted);
            }
        }

        public void Add(ExampleDataItem[] list)
        {
            if (list == null)
            {
                return;
            }

            foreach (var item in list)
            {
                items.Add(item);
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public ExampleDataItem ElementAt(int index)
        {
            return items.ElementAt(index);
        }

        public ExampleDataItem[] GetAll()
        {
            return items.ToArray();
        }

        public ExampleDataItem GetByPrimaryKey(object obj)
        {
            if (obj is Guid id)
            {
                return items.First(x => x.ID == id);
            }

            return null;
        }

        public Type GetGenericType()
        {
            return typeof(ExampleDataItem);
        }

        public int GetIndexOf(object obj)
        {
            return items.IndexOf((ExampleDataItem)obj);
        }

        public string GetTitle(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (obj is ExampleDataItem item)
            {
                return item.SomeString;
            }

            return obj.ToString();
        }


        private ObservableCollection<string> _titles = new ObservableCollection<string>();
        public ObservableCollection<string> GetTitles()
        {
            return _titles;
        }

        public void Remove(ExampleDataItem obj)
        {
            items.Remove(obj);
            RefillTitles();
        }


        private void RefillTitles()
        {
            _titles.Clear();
            foreach (var item in items)
            {
                _titles.Add(item.SomeString);
            }
        }

        public int GetIndexOf(ExampleDataItem obj)
        {
            return items.IndexOf(obj);
        }
    }
}
