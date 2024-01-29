using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class GetTokenViewModel
{
    [Required]
    public long ProviderId { get; set; }        
}
