namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ApplicationCallbackUrlReadModel
{
    public long Id { get; set; }
    public string CallbackUrl { get; set; }
    public long ApplicationId { get; set; }
    public ApplicationReadModel Application { get; set; }
}
