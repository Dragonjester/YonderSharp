using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YonderSharp.IdentityManagement
{
    /// <summary>
    /// Client-Side Token for server access
    /// </summary>
    [DataContract]
    public class Token
    {
        /// <summary>
        /// Unique ID of the user
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// E-Mail Adress of the user. Most likely unique.
        /// </summary>
        [DataMember]
        public string EMail { get; set; }

        /// <summary>
        /// Roles that the user has
        /// </summary>
        [DataMember]
        public HashSet<string> Claims { get; set; } = new HashSet<string>();

        /// <summary>
        /// Registration Date, UTC
        /// </summary>
        [DataMember]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Was the user a bad boy?
        /// </summary>
        [DataMember]
        public bool IsBanned { get; set; }

        /// <summary>
        /// Used for verification the integrity of the token. Don't change it, unless really appropiate!
        /// </summary>
        [DataMember]
        public string Verification { get; set; }

        /// <summary/>
        public Token(Guid id, string mail, HashSet<string> claims, DateTime createdAt, string verification)
        {
            this.UserId = id;
            this.EMail = mail;
            Claims = claims;
            CreatedAt = createdAt;
            Verification = verification;
        }

        /// <summary/>
        public Token()
        {
        }
    }
}
