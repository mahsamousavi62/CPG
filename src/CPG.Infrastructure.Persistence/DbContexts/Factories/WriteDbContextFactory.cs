using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CPG.Infrastructure.Persistence.DbContexts.Factories;

public class WriteDbContextFactory : IDesignTimeDbContextFactory<WriteDbContext>
{
    private const string ConnectionStringConfigName = "CPGConnectionString";

    public WriteDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString(ConnectionStringConfigName);

        var optionsBuilder = new DbContextOptionsBuilder<WriteDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new WriteDbContext(optionsBuilder.Options, null);
    }
}
