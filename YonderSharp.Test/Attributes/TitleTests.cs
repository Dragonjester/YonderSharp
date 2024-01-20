using NUnit.Framework;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.Test.Attributes
{
    [TestFixture]
    public class TitleTests
    {
        [Test]
        public void GetTitleTest()
        {
            Assert.AreEqual("Ente", Title.GetTitel(new TitleTestClass()));
        }
    }


    internal class TitleTestClass
    {
        [Title]
        public string MyTitle { get; set; } = "Ente";
    }
}
