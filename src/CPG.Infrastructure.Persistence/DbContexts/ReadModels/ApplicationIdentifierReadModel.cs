namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ApplicationIdentifierReadModel
{
    public long Id { get; set; }
    public string IdpClientId { get; set; }
    public long ApplicationId { get; set; }
    public ApplicationReadModel Application { get; set; }
}