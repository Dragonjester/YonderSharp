using FluentAssertions;
using NUnit.Framework;
using YonderSharp.Extensions;

namespace YonderSharp.Test.Extensions
{
    public class BoolExtensionsTests
    {
        [Test]
        public void IsTrueReturnsTrue()
        {
            bool? value = true;
            value.IsTrue().Should().BeTrue();
        }

        [Test]
        public void IsTrueReturnsFalse()
        {
            bool? value = null;
            value.IsTrue().Should().BeFalse();

            value = false;
            value.IsTrue().Should().BeFalse();
        }
    }
}
