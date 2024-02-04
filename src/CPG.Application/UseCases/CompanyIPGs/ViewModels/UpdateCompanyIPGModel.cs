using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class UpdateCompanyIPGModel:CreateCompanyIPGModel
{
    [Required]
    public long Id { get; set; }
}

