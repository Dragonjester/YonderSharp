using NUnit.Framework;
using NUnit.Framework.Legacy;
using YonderSharp.Attributes.DataManagement;

namespace YonderSharp.Test.Attributes
{
    [TestFixture]
    public class TitleTests
    {
        [Test]
        public void GetTitleTest()
        {
            ClassicAssert.AreEqual("Ente", Title.GetTitel(new TitleTestClass()));
        }
    }


    internal class TitleTestClass
    {
        [Title]
        public string MyTitle { get; set; } = "Ente";
    }
}
