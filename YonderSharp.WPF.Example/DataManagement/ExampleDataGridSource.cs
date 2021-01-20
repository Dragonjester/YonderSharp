using System;
using System.Collections.Generic;
using System.Linq;

namespace YonderSharp.WPF.DataManagement.Example
{
    public class ExampleDataGridSource : IDataGridSource
    {

        List<ExampleDataItem> _items = new List<ExampleDataItem>();

        public string _searchText { get; set; }

        public ExampleDataGridSource()
        {
            for (int i = 0; i < 100; i++)
            {
                _items.Add(CreateItem(i));
            }
        }

        private ExampleDataItem CreateItem(int i)
        {
            ExampleDataItem item = new ExampleDataItem();
            item.SomeBool = i % 2 == 0;
            item.SomeInt = i;
            item.SomeString = $"Item {i}";
            item.SomeFloat = i * 1.23456f;
            item.SomeLong = i + 1;
            item.SomeDouble = i * 3.454699;
            item.SomeDecimal = i * 4.56789m;

            return item;
        }

        public void AddItem(object item)
        {
            _items.Add((ExampleDataItem)item);
        }

        public object[] GetAddableItems()
        {
            List<object> addable = new List<object>();
            for (int i = _items.Count + 1; i < 200; i++)
            {
                addable.Add(CreateItem(i));
            }

            return addable.ToArray();
        }

        public string GetShownItemTitle(object item)
        {
            return ((ExampleDataItem)item).SomeString;
        }

        public Type GetTypeOfObjects()
        {
            return typeof(ExampleDataItem);
        }

        public bool IsFieldPartOfListText(string fieldName)
        {
            return fieldName == "SomeString";
        }

        public void RemoveShownItem(object item)
        {
            _items.Remove((ExampleDataItem)item);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public object[] GetAllItems()
        {
            return _items.ToArray();
        }

        public void AddNewItem()
        {
            _items.Add(new ExampleDataItem());
        }
    }
}
