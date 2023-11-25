using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CPG.API.Model;

public class CreateCompanyModel
{
    [Required]
    public  string PersianName { get; set; }
    [Required] 
    public required string EnglishName { get; set; }
    [Required]
    public bool NationalCodeMatchingRequied { get; set; }
    [Required]
    public required short[] MethodTypes { get; set; }
    [Required]
    public List<long> Users { get; set; }
}
