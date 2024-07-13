using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace YonderSharp.Test
{
    public class ParsingHelperTests
    {
        
        [Test]
        public void DoubleToStringTest()
        {
            ClassicAssert.AreEqual("51.12345", ParsingHelper.DoubleToString(51.12345));
            ClassicAssert.AreEqual("51.12346", ParsingHelper.DoubleToString(51.123456));
            ClassicAssert.AreEqual("51.12346", ParsingHelper.DoubleToString(51.1234567));
        }


        [Test]
        public void StringToDoubleTest()
        {
            ClassicAssert.AreEqual(51.12345, ParsingHelper.StringToDouble("51.12345"));
            ClassicAssert.AreEqual(51.12346, ParsingHelper.StringToDouble("51.123456"));
            ClassicAssert.AreEqual(51.12346, ParsingHelper.StringToDouble("51.1234567"));
        }
    }
}
