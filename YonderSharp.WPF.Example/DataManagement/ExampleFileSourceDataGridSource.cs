using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using YonderSharp.FileSources;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.DataManagement.Example;

namespace YonderSharp.WPF.Example.DataManagement
{
    public class ExampleFileSourceDataGridSource : DataGridFileSource
    {
        public ExampleFileSourceDataGridSource()
        {
            SetFileSource(new ExampleFileSource());
        }

        public override void AddNewItem()
        {
            var item = new ExampleDataItem2();
            item.SomeString = "New Item";
            AddItem(item);
        }

        /// <inheritdoc/>
        public virtual bool IsAllowedToAddFromList()
        {
            return false;
        }

        public override object[] GetAddableItems(IList<object> notAddableItems)
        {
            throw new NotImplementedException();
        }

        internal class ExampleFileSource : IFileSource<ExampleDataItem2>
        {
            public override string GetPathToJsonFile()
            {
                return Directory.GetCurrentDirectory() + "\\ExampleFileSourceDataGridSource\\ExmpleDataItem.json";
            }
        }

        [DataContract]
        public class ExampleDataItem2 : ExampleDataItem
        {

            [DataMember]
            public string SomeString2 { get; set; }
        }
    }
}