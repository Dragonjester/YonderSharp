using NUnit.Framework;
using System;
using System.Linq;
using YonderSharp.ThirdPartyAPIs.OverPass.Pulling;

namespace YonderSharp.ThirdPartyAPIs.Tests.OverPass.Pulling
{
    public class CityPullerTests
    {
        [Test]
        public void CityPullerTest()
        {
            CityPuller puller = new CityPuller();
            var result = puller.Pull(51, 7, 51, 7).ToArray();

            //exact count might change over time, so lets be vague
            Assert.IsTrue(result.Count() > 1000); 
            Assert.IsTrue(result.Any(x => x.Name == "Dortmund"));
        }
    }
}
