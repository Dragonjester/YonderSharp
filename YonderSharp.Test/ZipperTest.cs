using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            ClassicAssert.IsTrue(testValue == unzipped);
        }

        [Test]
        public void ZipUnzipToFromFile()
        {
            ZipContent original = new ZipContent();
            original.A = "A";
            original.B = 6;

            string pathToZip = AppDomain.CurrentDomain.BaseDirectory + "ZipUnzipToFromFile.zip";

            Zipper.Zip(original, pathToZip);
            ZipContent unziped = Zipper.Unzip<ZipContent>(pathToZip);

            ClassicAssert.AreEqual(original, unziped);
        }



    }

    [DataContract]
    public class ZipContent
    {
        [DataMember]
        public string A { get; set; }

        [DataMember]
        public int B { get; set; }

        [DataMember]
        public DateTime C { get; set; } = DateTime.UtcNow;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is ZipContent compareObj))
            {
                return false;
            }

            return A == compareObj.A && B == compareObj.B && C == compareObj.C;
        }
    }
}
