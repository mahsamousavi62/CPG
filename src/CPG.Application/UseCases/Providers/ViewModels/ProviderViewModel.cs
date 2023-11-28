using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Providers.ViewModels;

public class ProviderViewModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }    
    public string Logo { get; set; }
    public string ProviderData { get; set; }
    public ProviderType ProviderType { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
}
