using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp.IdentityManagement
{
    public interface ITokenHelper
    {
        public bool IsValid(Token token, string secret);
        public void SetVerification(Token token, string secret);
        public string CreateVerification(Token token, string secret);
        public Token CreateToken(User user, string secret);
    }
}
