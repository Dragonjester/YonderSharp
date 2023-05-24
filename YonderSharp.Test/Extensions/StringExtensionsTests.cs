using FluentAssertions;
using NUnit.Framework;
using YonderSharp.Extensions;

namespace YonderSharp.Test.Extensions
{
    public class StringExtensionsTests
    {
        [Test]
        public void HasContentDetectsContent()
        {
            "bla".HasContent().Should().BeTrue();
        }

        [Test]
        public void HasContentDetectsMissingContent()
        {
            "".HasContent().Should().BeFalse();

            string noContent = null;
            noContent.HasContent().Should().BeFalse();

            noContent = string.Empty;
            noContent.HasContent().Should().BeFalse();
        }
    }
}
