using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Providers.ViewModels;

public class UpdateProviderModel: CreateProviderModel
{
    [Required]
    public long Id { get; set; }
}
