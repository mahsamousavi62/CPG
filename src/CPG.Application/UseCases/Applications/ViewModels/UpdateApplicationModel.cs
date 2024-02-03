using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.Application.ViewModels;

public class UpdateApplicationModel:CreateApplicationModel
{
    [Required]
    public int Id { get; set; } 
}