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
            Assert.IsTrue(((IDataGridSource)testObject).IsFieldPartOfListText("SomeString"));
            Assert.IsFalse(((IDataGridSource)testObject).IsFieldPartOfListText("SomeLong"));

            //Some none-sense
            Assert.IsFalse(((IDataGridSource)testObject).IsFieldPartOfListText("Ente"));
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

            Assert.IsTrue(testObject.GetAllItems().All(x => ((ExampleDataItem)x).ID != newItem.ID));
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



        internal class DataGridFileSourceTestImplementation : DataGridFileSource
        {
            public override void AddNewItem()
            {
                AddItem(new ExampleDataItem());
            }

            public override object[] GetAddableItems(IList<object> notAddableItems)
            {
                throw new NotImplementedException();
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