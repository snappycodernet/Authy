using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities
{
    public class User
    {
        public int TenantId { get; set; }

        [AutoIncrement]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PIN { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public string Salt { get; set; }
        public long? LastModifiedUserId { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? LastModifiedTimestamp { get; set; }

        [Reference]
        public Tenant Tenant { get; set; }

        [Reference]
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}