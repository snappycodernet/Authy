using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Data.Models
{
    public class ApiKey
    {
        public int TenantId { get; set; }
        public string Id { get; set; }
        public string UserAuthId { get; set; }
        public string Environment { get; set; }
        public string KeyType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CancelledDate { get; set; }
        public string Notes { get; set; }
        public string RefIdStr { get; set; }
        public int RefId { get; set; }
        public string Meta { get; set; }
    }
}
