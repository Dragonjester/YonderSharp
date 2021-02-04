using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YonderSharp.IdentityManagement
{
    [DataContract]
    public class User
    {
        [DataMember]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [DataMember]
        public string EMail { get; set; }

        [DataMember]
        public DateTime RegistrationDateUtc { get; set; } = DateTime.UtcNow;

        [DataMember]
        public string Salt { get; set; }

        [DataMember]
        public string SaltedPasswordHash { get; set; }

        [DataMember]
        public bool IsBanned { get; set; }
    }
}
