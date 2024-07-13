using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using YonderSharp.FileSources;

namespace YonderSharp.Test
{
    public class IFileSourceTest
    {
        private FileHelperTestImplementation testObject;
        private string folder = Directory.GetCurrentDirectory() + "\\IFileSourceTest";
        private int countOfRaisedChangedEvents;


        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(folder);
            countOfRaisedChangedEvents = 0;

            testObject = new FileHelperTestImplementation(folder + "\\" + DateTime.UtcNow.Ticks + ".json");
            testObject.EntriesHaveChangedEvent += () =>
            {
                countOfRaisedChangedEvents++;
            };

        }

        [TearDown]
        public void TearDown()
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }
            Directory.Delete(folder);
        }

        [Test]
        public void SaveFileCanBeLoaded()
        {
            //TODO: this doesn't support the title check

            //string test = "ALLE MEINE ENTCHEN";
            //testObject.Add(test);
            //testObject.Save();

            //testObject.Remove(test);

            //testObject.Load(true);

            //ClassicAssert.AreEqual(1, testObject.GetAll().Length);
            //ClassicAssert.AreEqual(test, testObject.GetAll()[0]);
        }


        [Test]
        public void AddedItemsCanBeRemoved()
        {
            string test = "ALLE MEINE ENTCHEN";
            testObject.Add(test);
            testObject.Remove(test);
            ClassicAssert.AreEqual(0, testObject.GetAll().Length);
        }

        [Test]
        public void MultipleItemsAddedCanBeAccessed()
        {
            var entries = new[] { "1", "2", "3" };
            testObject.Add(entries);
            ClassicAssert.AreEqual(3, testObject.GetAll().Length);
        }


        [Test]
        public void AddedItemsCanBeAccessedTest()
        {
            string test = "ALLE MEINE ENTCHEN";
            testObject.Add(test);
            ClassicAssert.AreEqual(test, testObject.GetAll()[0]);
        }

        [Test]
        public void EntriesHaveChangedEventGetsCalledOnRemove()
        {
            AddedItemsCanBeRemoved();
            ClassicAssert.AreEqual(2, countOfRaisedChangedEvents);
        }

        [Test]
        public void EntriesHaveChangedEventGetsCalledOnAdd()
        {
            AddedItemsCanBeAccessedTest();
            ClassicAssert.AreEqual(1, countOfRaisedChangedEvents);
        }

        [Test]
        public void EntriesHaveChangedEventGetsCalledOnAddMultiple()
        {
            MultipleItemsAddedCanBeAccessed();
            ClassicAssert.AreEqual(3, countOfRaisedChangedEvents);
        }
    }

    internal class FileHelperTestImplementation : IFileSource<string>
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
