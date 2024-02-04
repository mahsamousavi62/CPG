using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyDeposits.ViewModels;

public class SetAsDefaultForDDViewModel
{
    [Required]
    public long CompanyId { get; set; }
    [Required]
    public long CompanyDepositId { get; set; }
}
