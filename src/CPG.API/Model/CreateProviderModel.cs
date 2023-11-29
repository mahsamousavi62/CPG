using System.ComponentModel.DataAnnotations;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.API.Model;

public class CreateProviderModel
{
    [Required]
    public required string PersianName { get; set; }

    [Required]
    public required string EnglishName { get; set; }

    [Required]
    public required ProviderType ProviderType { get; set; }

    [Required]
    public required string ProviderData { get; set; }

    [Required]
    public required IFormFile File { get; set; }
}
