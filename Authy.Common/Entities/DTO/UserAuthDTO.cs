using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities.DTO
{
    public class UserAuthDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
