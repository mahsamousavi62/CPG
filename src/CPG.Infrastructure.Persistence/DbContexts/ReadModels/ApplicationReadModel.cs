using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ApplicationReadModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public string Logo { get; set; }
    public string ResponseApiUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public IEnumerable<ApplicationIdentifierReadModel> ApplicationIdentifiers { get; set; }
    public IEnumerable<ApplicationCallbackUrlReadModel> ApplicationCallbackUrls { get; set; }
    public ICollection<PaymentRequestReadModel> PaymentRequests{ get; set; }
}