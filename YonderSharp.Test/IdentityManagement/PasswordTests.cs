using NUnit.Framework;
using System.Collections.Generic;
using YonderSharp.IdentityManagement;

namespace YonderSharp.Test.IdentityManagement
{
    [TestFixture]
    public class PasswordTests
    {
        [Test]
        public void TestPasswordValidationWorks()
        {
            string password = "1234Password;";
            string salt = "ölaksdjhföloawie4höojkahdfg";

            PasswordHelper testObject = new PasswordHelper(salt.Length);
            string pwHash = testObject.CreatePasswordHash(password, salt);
            Assert.IsTrue(testObject.IsPasswordCorrect(password, salt, pwHash));
        }

        [Test]
        public void TestSaltUniqueness()
        {
            int saltLength = 24;
            PasswordHelper testObject = new PasswordHelper(saltLength);
            HashSet<string> salts = new HashSet<string>();
            for(int i = 0; i < 10000000; i++)
            {
                string salt = testObject.CreateSalt();
                Assert.IsFalse(string.IsNullOrWhiteSpace(salt));
                Assert.IsTrue(salts.Add(salt));
            }
        }

    }
}
