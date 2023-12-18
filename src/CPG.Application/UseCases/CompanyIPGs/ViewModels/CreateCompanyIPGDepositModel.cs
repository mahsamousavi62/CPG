using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels
{
    public class CreateCompanyIPGDepositModel
    {
        [Required]
        public long DepositId { get; set; }

        [Required]
        public bool IsDefault { get; set; }
    }
}
