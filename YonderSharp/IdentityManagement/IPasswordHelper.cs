namespace YonderSharp.IdentityManagement
{
    /// <summary>
    /// Helping methods for Password related operations
    /// </summary>
    public interface IPasswordHelper
    {
        /// <summary>
        /// Creates an entirely new salt
        /// </summary>
        public string CreateSalt();


        /// <summary>
        /// Creates the hash for the combination of the password and salt. 
        /// </summary>
        /// <param name="pwd">Password as provided by the user</param>
        /// <param name="salt">Salt (hopefully) provided by the system (db?)</param>
        /// <returns>the string representation of the hash</returns>
        public string CreatePasswordHash(string pwd, string salt);


        /// <summary>
        /// Checks if a password is valid
        /// </summary>
        public bool IsPasswordCorrect(string providedPassword, string salt, string expectedPasswordHash);

    }
}
