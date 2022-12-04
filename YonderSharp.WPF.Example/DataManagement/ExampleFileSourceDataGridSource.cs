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


        private DataGridSourceConfiguration _config;
        protected override DataGridSourceConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                _config = new DataGridSourceConfiguration();
                _config.IsAllowedToIsAllowedToAddFromList = false;
                _config.IsAllowedToCreateNewEntry = true;
                _config.IsAllowedToRemove = true;
                _config.HasSearch = true;
                _config.GetAddableItemsReturnAll = true;
            }

            return _config;
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