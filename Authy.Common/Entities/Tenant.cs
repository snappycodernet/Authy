using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Entities
{
    [Schema("common")]
    public class Tenant
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}