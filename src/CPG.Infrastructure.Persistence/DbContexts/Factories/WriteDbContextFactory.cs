using CPG.Domain.SharedKernel.Logging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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

        // Create mock/null implementations for design-time only
        var mediatorMock = new Mock<IMediator>();
        var loggerMock = new NullLogger<WriteDbContext>();
        var auditLogServiceMock = new Mock<IAuditLogService>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        return new WriteDbContext(
            optionsBuilder.Options,
            mediatorMock.Object,
            loggerMock,
            auditLogServiceMock.Object,
            httpContextAccessorMock.Object);
    }
}
