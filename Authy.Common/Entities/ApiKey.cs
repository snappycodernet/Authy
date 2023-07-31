using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities
{
    public class ApiKey
    {
        public long TenantId { get; set; }

        [AutoIncrement]
        public long Id { get; set; }
        public long UserAuthId { get; set; }
        public string Environment { get; set; }
        public string KeyType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string Notes { get; set; }
        public string RefIdStr { get; set; }
        public int? RefId { get; set; }
        public string Meta { get; set; }
    }
}