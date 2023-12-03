using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels
{
    public class UserReadModel
    {
        public long Id { get; set; }
        public string IDPId { get; }
        public string NationalCode { get; }
        public long? CompanyId { get; set; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PhoneNumber { get; }
        public DateTime LastUpdateFromIDP { get; set; }
        public short KYCStatus { get; }
        public bool IsLegal { get; }
        public bool IsActive { get; }
    }
}
