using NUnit.Framework;
using YonderSharp.Geocaching;

namespace YonderSharp.Test.Geocaching
{
    [TestFixture]
    public class EhemaligerLandKreisWaldeckTests
    {
        EhemaligerLandkreisWaldeck solver;

        public EhemaligerLandKreisWaldeckTests()
        {
            solver = new EhemaligerLandkreisWaldeck();
        }

        [Test]
        [TestCase("N LI° XXV. CCCXIV O IX° III.DCLVIII", ExpectedResult = "N51° 25.314 E9° 3.658", TestName = "Roman Numbers - GC7HTAN")]
        public string TestRomanNumerals(string romanText)
        {
            return solver.Solve(romanText);
        }

        [Test]
        public void ColorPuzzlesTest()
        {
            Assert.AreEqual("N51° 29.362 E09° 00.479", solver.Solve("N  grün braun°  rot weiß.orange blau rot O  schwarz weiß°  schwarz schwarz.gelb violett weiß"));

        }

        [Test]
        public void KeyBoardTest()
        {
            Assert.AreEqual("N51° 28.714 E09° 02.384", solver.Solve("N  % !°   \"(.   / ! $ O  = )°   = \".   § ($"));
        }

        [Test]
        [TestCase("N üfnf snie° wize hecss. sine csehs wize O llnu unne° luln iver. revi ulnl seni", ExpectedResult = "N51° 26.162 E09° 04.401", TestName = "Anagramme - GC7HR9M")]
        [TestCase("N  ffnü nies° izew üffn. eunn csehs irde O  luln nune° lnlu iedr. zewi nebies sien", ExpectedResult = "N51° 25.963 E09° 03.271", TestName = "Anagramme - GC7HT6B")]
        public string AnagrammTest(string text)
        {
            return solver.Solve(text);
        }

        [Test]
        [TestCase("N afgcd bc°  bc fgbc.  fedcg abged bc O  afedcb abcdefg°  abgcd abgcd.  abged fedcg bc", ExpectedResult = "N51° 14.621 E08° 33.261", TestName = "7Segment - GC7Q7ZM")]
        [TestCase("N afgcd bc°  abged afgbc.  bc abc fgbc O afedcb afgbc°  afedcb afedcb.  abgcd afgcd fgbc", ExpectedResult = "N51° 29.174 E09° 00.354", TestName = "7Segment - GC7H978")]
        [TestCase("N afgcd bc°  abgcd abcdef.  abgcd abcdefg bc O  afedbc afgbc° afedcb afgcd. bc abged abgde", ExpectedResult = "N51° 30.381 E09° 05.122", TestName = "7Segment - GC7HH1M")]
        [TestCase("N afgcd bc° abged afgbc. abgcd abgcd fedcg O afedcb afgbc° afedcb fgbc. afgcdeb abgcd afgbc", ExpectedResult = "N51° 29.336 E09° 04.839", TestName = "7Segment - GC7HNB0")]
        public string SevenSegment(string text)
        {
            return solver.Solve(text);
        }
    }
}
