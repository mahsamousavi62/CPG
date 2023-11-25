using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.ViewModels;

public class CompanyViewModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public bool NationalCodeMatchingRequied { get; set; }
    public string Logo { get; set; }
    public FileViewModel     FileViewModel { get; set; }
    public Dictionary<short, string> PaymentMethods { get; set; }
}
