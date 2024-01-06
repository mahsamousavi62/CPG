using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Users.ViewModel
{
    public class UserAuthenticateViewModel: UserViewModel
    {
        public long? CompanyId { get; set; }
        public long? ApplicationId { get; set; }
        public Enums.AuditType AuditType { get; set; }
    }
}
