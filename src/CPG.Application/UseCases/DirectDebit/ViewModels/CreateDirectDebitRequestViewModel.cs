using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class CreateDirectDebitRequestViewModel
{
    [Required]
    public int BankId { get; set; }
}
