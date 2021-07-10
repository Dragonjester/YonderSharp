using NUnit.Framework;

namespace YonderSharp.Test
{
    public class ParsingHelperTests
    {
        
        [Test]
        public void DoubleToStringTest()
        {
            Assert.AreEqual("51.12345", ParsingHelper.DoubleToString(51.12345));
            Assert.AreEqual("51.12346", ParsingHelper.DoubleToString(51.123456));
            Assert.AreEqual("51.12346", ParsingHelper.DoubleToString(51.1234567));
        }


        [Test]
        public void StringToDoubleTest()
        {
            Assert.AreEqual(51.12345, ParsingHelper.StringToDouble("51.12345"));
            Assert.AreEqual(51.12346, ParsingHelper.StringToDouble("51.123456"));
            Assert.AreEqual(51.12346, ParsingHelper.StringToDouble("51.1234567"));
        }
    }
}
