using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.Companies.ViewModels;

public class CreateCompanyModel
{
    [Required]
    public required string PersianName { get; set; }

    [Required] 
    public required string EnglishName { get; set; }

    [Required]
    public required bool NationalCodeMatchingRequired { get; set; }

    [Required]
    public string SiteAddress { get; set; }

    [Required]
    public Enums.IpgRedirectionMethodType IpgRedirectionMethodType { get; set; }
    [Required]
    public required List<Enums.PaymentMethodType> MethodTypes { get; set; }
    [Required]
    public required List<long> Users { get; set; }

    [Required]
    public required IFormFile File { get; set; }

    public string Key { get; set; }

    public string Iv { get; set; }

    public int? ThirdPartyCode { get; set; }
}
