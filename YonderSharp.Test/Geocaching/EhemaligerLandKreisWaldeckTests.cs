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
        [TestCase("N LI° XXIX. CMXIV E VIII° LVII. CCCLXXXIV", ExpectedResult = "N51° 29.914 E8° 57.384", TestName = "Roman Numbers - GC7H1ZX")]
        //lets wait if there are more with that syntax before implementing it...
        //[TestCase("N V I° II IX.II I IV E VIII° V III.IX II V", ExpectedResult = "N51° 29.214 E8° 53.925", TestName = "Roman Numbers - GC7GQD2")]


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
        [TestCase("N  % !°   \"(.   / ! $ O  = )°   = \".   § ($", ExpectedResult = "N51° 28.714 E09° 02.384", TestName = "ShiftNumber - GC7H9VR")]
        public string KeyBoardTest(string text)
        {
            return solver.Solve(text);
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
        [TestCase("N  afgcd bc°  abged afgbc.  abcdefg abc abcdef\r\nO  abcdef afgbc°  abcdef abcdef.  abgcd afgcd afedcg", ExpectedResult = "N51° 29.870 E09° 00.356", TestName = "7Segment - GC7H95Y")]
        
        public string SevenSegment(string text)
        {
            return solver.Solve(text);
        }

        [Test]
        [TestCase("N  7,4,8,9,6,1,0,3,2.  9,0,3,2,7,5,8,6,4.°  5,8,4,0,1,6,7,9,2.  6,5,8,4,2,7,1,3,9..  4,7,3,0,5,9,2,8,6.  2,6,5,8,4,3,9,0,1.  1,2,9,6,3,8,0,4,7.\r\n\r\nO  4,2,1,7,9,6,8,3,5.  0,4,8,2,5,3,1,6,7.°  1,5,9,7,8,2,3,4,6.  3,9,1,7,5,2,4,8,0..  2,6,0,8,1,7,5,9,4.  6,1,0,3,8,4,5,7,9.  4,5,1,8,3,0,7,6,2.", ExpectedResult = "N51° 30.175 E09° 06.329", TestName ="MissingNumber - GC7HMZP")]
        public string MissingNumber(string text)
        {
            return solver.Solve(text);
        }
    }
}
