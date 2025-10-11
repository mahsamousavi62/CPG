using CPG.Domain.SharedKernel.Logging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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

        // Create null implementations for design-time only (migrations)
        // These are never used during migrations, only constructor is needed
        IMediator mediator = null!;
        ILogger<WriteDbContext> logger = NullLogger<WriteDbContext>.Instance;
        IAuditLogService auditLogService = null!;
        IHttpContextAccessor httpContextAccessor = null!;

        return new WriteDbContext(
            optionsBuilder.Options,
            mediator,
            logger,
            auditLogService,
            httpContextAccessor);
    }
}
