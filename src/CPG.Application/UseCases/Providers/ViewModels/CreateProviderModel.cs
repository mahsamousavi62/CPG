using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Providers.ViewModels;

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

    [Required]
    public short IpgVerificationTimeLimit { get; set; }

    [Required]
    public string IpgBaseUrl { get; set; }
}
