using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Models
{
    public class CodeModel
    {
        public int Id { get; set; }
        public string UserLogin { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public bool IsUsed { get; set; }
    }
}
