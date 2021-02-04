using System;
using System.Security.Cryptography;
using System.Text;

namespace YonderSharp.IdentityManagement
{
	/// <inheritdoc cref="IPasswordHelper"/>
    public class PasswordHelper : IPasswordHelper
	{
		private int _saltSize;

		/// <param name="saltSize">Length of the salt to use</param>
		public PasswordHelper(int saltSize)
        {
			_saltSize = saltSize;

			if(saltSize <= 12)
            {
				throw new Exception($"Dude... {saltSize}... really?!");
            }
        }

		/// <inheritdoc cref="IPasswordHelper"/>
		public string CreateSalt()
		{
			using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
			{
				byte[] buff = new byte[_saltSize];
				rng.GetBytes(buff);
				return Convert.ToBase64String(buff);
			}
		}

		/// <inheritdoc cref="IPasswordHelper"/>
		public string CreatePasswordHash(string pwd, string salt)
		{
			// Die verwendetet Salt-Länge für SHA1 war 5 Byte (entspricht 8 Base64-Zeichen).
			// Alles was einen längeren Salt-Wert hat ist folglich ein SHA512-Hash. 
			bool sha512 = salt?.Length > 8;

			string pwdAndSalt = string.Concat(pwd, salt);

			using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA-512"))
			{				
				return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt)));
			}
		}

		/// <inheritdoc cref="IPasswordHelper"/>
		public bool IsPasswordCorrect(string providedPassword, string salt, string expectedPasswordHash)
		{
			if (string.IsNullOrEmpty(providedPassword))
			{
				return false;
			}

			if (string.IsNullOrEmpty(salt))
			{
				return false;
			}

			if (string.IsNullOrEmpty(expectedPasswordHash))
			{
				return false;
			}

			return expectedPasswordHash == CreatePasswordHash(providedPassword, salt);
		}
	}
}
