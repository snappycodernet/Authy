using Authy.Common.Entities;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities
{
    public class UserRole
    {
        public int TenantId { get; set; }

        [AutoIncrement]
        public int Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? LastModifiedTimestamp { get; set; }

        [Reference]
        public Role Role { get; set; }
    }
}
