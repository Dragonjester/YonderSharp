using FluentAssertions;
using NUnit.Framework;

namespace YonderSharp.Test
{
    [TestFixture]
    public class LevenshteinDistanceCalculatorTests
    {
        private LevenshteinDistanceCalculator _calculator;
        
        [SetUp]
        public void SetUp()
        {
            _calculator = new LevenshteinDistanceCalculator();
        }

        [TestCase("a", "a", 0)]
        [TestCase("a", "b", 1)]
        [TestCase("A", "a", 1)]
        [TestCase("AB", "ab", 2)]
        [TestCase("AB", "Ab", 1)]
        [TestCase("ABC", "Ab", 2)]
        [TestCase(null, "a", 1)]
        [Test]
        public void TestNormalDistance(string first, string second, int expectation)
        {
            _calculator.CalculateLevenshteinDistance(first, second).Should().Be(expectation);
            _calculator.CalculateLevenshteinDistance(second, first).Should().Be(expectation);
        }


        [TestCase("a", "b", 1)]
        [TestCase("A", "a", 0)]
        [TestCase("a", "a", 0)]
        [TestCase("AB", "ab", 0)]
        [TestCase("AB", "Ab", 0)]
        [TestCase("ABC", "Ab", 1)]
        [TestCase(null, "a", 1)]
        [Test]
        public void TestIgnoredCaseDistance(string first, string second, int expectation)
        {
            _calculator.CalculateLevenshteinDistanceIgnoringCase(first, second).Should().Be(expectation);
            _calculator.CalculateLevenshteinDistanceIgnoringCase(second, first).Should().Be(expectation);
        }
    }
}
