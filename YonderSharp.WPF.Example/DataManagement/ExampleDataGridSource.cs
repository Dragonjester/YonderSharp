using System;
using System.Collections.Generic;

namespace YonderSharp.WPF.DataManagement.Example
{
    public class ExampleDataGridSource : IDataGridSource
    {

        private string _prefix;
        public ExampleDataGridSource(string prefix = "Item")
        {
            _prefix = prefix;
            for (int i = 0; i < 100; i++)
            {
                _items.Add(CreateItem(prefix, i));
            }
        }

        private ExampleDataItem CreateItem(string prefix, int i)
        {
            ExampleDataItem item = new ExampleDataItem();
            item.SomeBool = i % 2 == 0;
            item.SomeInt = i;
            item.SomeString = $"{prefix} {i}";
            item.SomeFloat = i * 1.23456f;
            item.SomeLong = i + 1;
            item.SomeDouble = i * 3.454699;
            item.SomeDecimal = i * 4.56789m;

            return item;
        }

        /// <inheritdoc/>
        public override Type GetTypeOfObjects()
        {
            return typeof(ExampleDataItem);
        }

        protected override void Save(IList<object> items)
        {
            //Do nothing
        }
    }
}