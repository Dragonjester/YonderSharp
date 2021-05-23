using System;
using System.Runtime.Serialization;

namespace YonderSharp.IdentityManagement
{
    /// <summary>
    /// Representation of a user
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Unique ID of the user
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Maybe (!) unique E-Mail Adress of the user
        /// </summary>
        [DataMember]
        public string EMail { get; set; }

        /// <summary>
        /// When did the user register?
        /// </summary>
        [DataMember]
        public DateTime RegistrationDateUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Salt of the PasswordHash
        /// </summary>
        [DataMember]
        public string Salt { get; set; }

        /// <summary>
        /// Salted PasswordHash
        /// </summary>
        [DataMember]
        public string SaltedPasswordHash { get; set; }

        /// <summary>
        /// Was the user a bad boy?
        /// </summary>
        [DataMember]
        public bool IsBanned { get; set; }

        /// <summary>
        /// As long as this is != null, the user is not fully registered!
        /// </summary>
        [DataMember]
        public Guid VerificationId { get; set; } = Guid.NewGuid();
    }
}
