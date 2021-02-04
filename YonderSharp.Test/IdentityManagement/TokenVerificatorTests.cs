using NUnit.Framework;
using System.Collections.Generic;
using YonderSharp.IdentityManagement;

namespace YonderSharp.Test.IdentityManagement
{
    [TestFixture]
    public class TokenVerificatorTests
    {
        private string _secret = "VerySecureSecret12345;";
        private ITokenHelper _tokenHelper = new TokenHelper();

        private Token GetToken()
        {
            Token token = new Token();
            token.Claims.Add("A");
            token.Claims.Add("B");
            token.Claims.Add("C");
            token.UserId = new System.Guid("96a8c274-b5d9-4cf1-8e61-75f2ef1ee40e");
            token.CreatedAt = new System.DateTime(2021, 02, 04, 20, 47, 5);
            token.EMail = "some@mail.com";
            token.Verification = "DefinitlyInvalidToken";

            return token;
        }

        [Test]
        public void TokenFromUserCreationTest()
        {
            User user = new User();
            user.EMail = "asdf@basdf.de";
            user.UserId = new System.Guid("96a8c274-b5d9-4cf1-8e61-75f2ef1ee40e");
            user.RegistrationDateUtc = new System.DateTime(2021, 02, 04, 20, 47, 5);
            user.Salt = "1^23123123123q123";
            user.SaltedPasswordHash = "234234234";

            Token token = _tokenHelper.CreateToken(user, _secret);
            Assert.IsTrue(_tokenHelper.IsValid(token, _secret));
        }

        [Test]
        public void InvalidVerificationisInvalidTest()
        { 
            //manipulated token needs to be invalid
            Assert.IsFalse(_tokenHelper.IsValid(GetToken(), _secret)); 
        }

        [Test]
        public void VerificationMethodDidNotChangeTest()
        {
            string expected = "03B57EE1388029EBABB6671CD9738F90BF6621797D7368D2F483E62AB40541373A4A0F4EFEAD1D918EB8B34108E39DDB88FA207E5FC8C5B3BEB32599952E7F12";
            Assert.AreEqual(expected, _tokenHelper.CreateVerification(GetToken(), _secret));
        }

        [Test]
        public void VerificationGetsCorrectlyAssigendTest()
        {
            Token token = GetToken();
            _tokenHelper.SetVerification(token, _secret);

            string expected = "03B57EE1388029EBABB6671CD9738F90BF6621797D7368D2F483E62AB40541373A4A0F4EFEAD1D918EB8B34108E39DDB88FA207E5FC8C5B3BEB32599952E7F12";
            Assert.AreEqual(expected, token.Verification);
        }

        [Test]
        public void GeneratedVerificationIsValidTest()
        {
            Token token = GetToken();
            token.Verification = _tokenHelper.CreateVerification(token, _secret);
            Assert.IsTrue(_tokenHelper.IsValid(token, _secret));
        }
    }
}
