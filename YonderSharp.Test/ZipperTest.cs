using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace YonderSharp.Test
{
    [TestFixture]
    public class ZipperTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void ZippedCanBeUnzipedTest()
        {
            string testValue = "";
            for (int i = 0; i < 100; i++)
            {
                testValue += DateTime.Now.Ticks.ToString();
            }

            var zipped = Zipper.Zip(testValue);

            var unzipped = Zipper.Unzip(zipped);
            Assert.IsTrue(testValue == unzipped);
        }
    }
}
