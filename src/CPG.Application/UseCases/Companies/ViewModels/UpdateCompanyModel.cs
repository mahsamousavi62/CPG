using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.Companies.ViewModels;

public class UpdateCompanyModel: CreateCompanyModel
{
    [Required]
    public long Id { get; set; }
}
