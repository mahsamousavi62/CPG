namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyPaymentMethodsReadModel
{
    public long Id { get; set; }
    public byte MethodType { get; set; }
    public long CompanyId { get; set; }
    public CompanyReadModel Company { get; set; }
}

