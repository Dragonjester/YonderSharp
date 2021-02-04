using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace YonderSharp.IdentityManagement
{
    public class TokenHelper : ITokenHelper
    {
        public bool IsValid(Token token, string secret)
        {
            if (secret == null)
            {
                return false;
            }

            if (token == null)
            {
                return false;
            }

            return token.Verification == CreateVerification(token, secret);

        }

        public void SetVerification(Token token, string secret)
        {
            token.Verification = CreateVerification(token, secret);
        }

        public string CreateVerification(Token token, string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            PropertyInfo[] infos = typeof(Token).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty);
            if (infos.Length != 6)
            {
                throw new InvalidOperationException($"did the token class change?! {infos.Length}");
            }

            string tokenVal = secret + token.CreatedAt.ToString() + token.IsBanned + token.EMail + string.Join(",", token.Claims);

            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA-512"))
            {
                return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(tokenVal))).Replace(" ", "").Replace("-", "");
            }
        }

        public Token CreateToken(User user, string secret)
        {
            Token token = new Token();
            token.UserId = user.UserId;
            token.CreatedAt = user.RegistrationDateUtc;
            token.Claims.Add("Registered");
            token.EMail = user.EMail;
            token.Verification = CreateVerification(token, secret);

            return token;
        }
    }
}