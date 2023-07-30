using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities.DTO
{
    public class UserCreationDTO
    {
        public int TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PIN { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
