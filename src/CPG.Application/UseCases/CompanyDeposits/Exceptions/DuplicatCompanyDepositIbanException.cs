using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.CompanyDeposits.Exceptions
{
    public class DuplicatCompanyDepositIbanException(string iban)
        : AppException(string.Format(GlobalResource.DuplicateIban, iban))
    {
        public override string Code => "duplicate_CompanyDeposit_iban";
        public string Iban { get; } = iban;
    }
}
