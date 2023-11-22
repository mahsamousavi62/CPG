using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyReadModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public string NationalCodeMatchingRequied { get; set; }
    public string Logo { get; set; }
    public List<CompanyPaymentMethods> PaymentMethods { get; set; }
}
