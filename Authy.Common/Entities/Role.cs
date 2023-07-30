using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities
{
    [Schema("common")]
    public class Role
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCoreRole { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? LastModifiedTimestamp { get; set; }
    }
}
