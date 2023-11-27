namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class BankReadModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IbanPrefix { get; set; }
    public bool IsActive { get; set; }
    public string LogoAddress { get; set; }
}
