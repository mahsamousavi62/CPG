using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CPG.API.Model;

public class CreateCompanyModel
{
    [Required]
    public required string PersianName { get; set; }

    [Required] 
    public required string EnglishName { get; set; }

    [Required]
    public required bool NationalCodeMatchingRequied { get; set; }

    [Required]
    public required short[] MethodTypes { get; set; }

    [Required]
    public required List<long> Users { get; set; }

    [Required]
    public required IFormFile File { get; set; }
}
