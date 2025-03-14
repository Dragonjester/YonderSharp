using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YonderSharp.FileSources;
using YonderSharp.WPF.DataManagement;
using YonderSharp.WPF.Test.DataTypes;

namespace YonderSharp.WPF.Test
{
    public class DataGridFileSourceTests
    {
        private FileHelperTestImplementation testFileSource;
        private DataGridFileSourceTestImplementation testObject;
        private string folder = Directory.GetCurrentDirectory() + "\\IFileSourceTest";
        private int countOfRaisedChangedEvents;


        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(folder);
            countOfRaisedChangedEvents = 0;

            LinkNewInstances();
            testObject.AddNewItem();
        }

        string ticks;
        private void LinkNewInstances()
        {
            if(ticks == null)
            {
                ticks = DateTime.UtcNow.Ticks.ToString();
            }

            testFileSource = new FileHelperTestImplementation(folder + "\\" + ticks + ".json");
            testFileSource.EntriesHaveChangedEvent += () =>
            {
                countOfRaisedChangedEvents++;
            };

            testObject = new DataGridFileSourceTestImplementation();
            testObject.SetFileSource(testFileSource);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }
            Directory.Delete(folder);

            ticks = null;
        }

        [Test]
        public void IsFieldPartOfListTextTest()
        {
            Assert.That(((IDataGridSource)testObject).IsFieldPartOfListText("SomeString"));
            Assert.That(!((IDataGridSource)testObject).IsFieldPartOfListText("SomeLong"));

            //Some none-sense
            Assert.That(!((IDataGridSource)testObject).IsFieldPartOfListText("Ente"));
        }

        [Test]
        public void AddingSingleItemTest()
        {
            ExampleDataItem newItem = new ExampleDataItem();
            newItem.SomeString = "asdf";
            testObject.AddItem(newItem);

            testObject.GetAllItems().First(x => ((ExampleDataItem)x).ID == newItem.ID);
        }

        [Test]
        public void RemoveItemTest()
        {
            ExampleDataItem newItem = new ExampleDataItem();
            newItem.SomeString = "newItem1";

            testObject.AddItem(newItem);
            testObject.RemoveShownItem(newItem);

            Assert.That(testObject.GetAllItems().All(x => ((ExampleDataItem)x).ID != newItem.ID));
        }


        [Test]
        public void SaveLoadTest()
        {
            ExampleDataItem newItem = new ExampleDataItem();
            newItem.SomeString = "newItem1";

            testObject.AddItem(newItem);
            
            testObject.Save();

            LinkNewInstances();

            testObject.GetAllItems().First(x => ((ExampleDataItem)x).ID == newItem.ID);
        }



        internal class DataGridFileSourceTestImplementation : DataGridFileSource<ExampleDataItem>
        {

            private DataGridSourceConfiguration _config;
            public override DataGridSourceConfiguration GetConfiguration()
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
        }

        internal class FileHelperTestImplementation : IFileSource<ExampleDataItem>
        {
            string myPath;
            public FileHelperTestImplementation(string path)
            {
                myPath = path;
            }

            public override string GetPathToJsonFile()
            {
                return myPath;
            }
        }
    }
}