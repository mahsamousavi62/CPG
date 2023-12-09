using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.Application.ViewModels;

public class CreateApplicationModel
{
    [Required]
    public required string PersianName { get; set; }

    [Required]
    public required string EnglishName { get; set; }

    [Required]
    public required string ResponseApiUrl { get; set; }

    [Required]
    public required IFormFile File { get; set; }

    [Required]
    public required string[] IdpClientIds { get; set; }

    public string[] CallbackUrls { get; set; }
}