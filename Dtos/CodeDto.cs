using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comander.Dtos
{
    public class CodeDto
    {
        public string Idc { get; set; }
        public string UserId { get; set; }
        public string UserLogin { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public string TypeOfCode { get; set; }
        public string AdditionalInfo { get; set; }
        public bool WasUsed { get; set; }
        public DateTime UsingDate { get; set; }
        public string CodeBeneficient { get; set; }
        public bool IsActive { get; set; }
    }
}
