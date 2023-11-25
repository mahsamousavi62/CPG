using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.Companies.Commands.Create;

public class CreateCompanyViewModel
{
    [Required]
    public string PersianName { get; set; }
    [Required]
    public string EnglishName { get; set; }
    [Required]
    public bool NationalCodeMatchingRequied { get; set; }
    [Required]
    public UploadFileViewModel UploadFile { get; set; }
    [Required]
    public short[] MethodTypes { get; set; }
}