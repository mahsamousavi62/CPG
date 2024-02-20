using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyDeposits.ViewModels;

public class SetAsDefaultForDDViewModel
{
    [Required]
    public long CompanyDepositId { get; set; }
}
