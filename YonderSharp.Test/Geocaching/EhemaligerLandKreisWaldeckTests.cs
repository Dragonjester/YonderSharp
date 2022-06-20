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
        public void AnagrammTest()
        {
            Assert.AreEqual("N51° 28.837 E09° 02.010", solver.Solve("N üfnf sein° wezi tahc. tach ride nesibe O lunl unen° nlul eizw.llun sein unll"));
        }
    }
}
