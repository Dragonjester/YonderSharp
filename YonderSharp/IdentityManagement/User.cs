using System;
using System.ComponentModel.DataAnnotations;
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
        /// Unique E-Mail Adress of the user
        /// </summary>
        [Key]
        [DataMember]
        public string EMail { get; set; }

        /// <summary>
        /// Unique ID of the user
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; } = Guid.NewGuid();

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

        [DataMember]
        public DateTime? BannedOnUtc { get; set; }

        [DataMember]
        public string BanReason { get; set; }
    }
}
