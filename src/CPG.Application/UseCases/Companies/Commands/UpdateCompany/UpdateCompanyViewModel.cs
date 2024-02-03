using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Commands.CreateCompany;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;

namespace CPG.Application.UseCases.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyViewModel : CreateCompanyViewModel
    {
        public UpdateCompanyViewModel(long id,  string persianName, string englishName, bool nationalCodeMatchingRequied, IFile file,
            Enums.PaymentMethodType[] methodTypes, List<long> users, string siteAddress,
            Enums.IpgRedirectionMethodType ipgRedirectionMethodType,
            string key, string iv, int? thirdPartyCode) : base(persianName, englishName, nationalCodeMatchingRequied, file, 
                methodTypes, users, siteAddress, ipgRedirectionMethodType, key, iv, thirdPartyCode)
        {
           Id = id;
        }
        public long Id { get; set; } 
    }
}
