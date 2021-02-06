using NUnit.Framework;
using System;
using System.Linq;
using YonderSharp.ThirdPartyAPIs.OverPass.Pulling;

namespace YonderSharp.ThirdPartyAPIs.Tests.OverPass.Pulling
{
    public class ShopPullerTests
    {
        [Test]
        public void ShopPullerTest()
        {
            ShopPuller puller = new ShopPuller();
            var result = puller.Pull(51,9,51,9).ToArray();
            //exact number can vary, due to changes in the real world :D
            Assert.IsTrue(result.Length > 9500);

            Assert.IsTrue(result.Any(x => x.Name.Contains("Alibaba")));
        }
    }
}
