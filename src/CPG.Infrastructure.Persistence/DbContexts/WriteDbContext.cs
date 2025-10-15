using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class WriteDbContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ILogger<WriteDbContext> _logger;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WriteDbContext(
        DbContextOptions<WriteDbContext> options,
        IMediator mediator,
        ILogger<WriteDbContext> logger,
        ILogService logService,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _mediator = mediator;
        _logger = logger;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyPaymentMethod> CompanyPaymentMethods { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<CompanyDeposit> CompanyDeposits { get; set; }
    public DbSet<Domain.AggregateModels.ApplicationAggregate.Application> Applications { get; set; }
    public DbSet<ApplicationIdentifier> ApplicationIdentifiers { get; set; }
    public DbSet<ApplicationCallbackUrl> ApplicationCallbackUrls { get; set; }
    public DbSet<IPGType> IPGTypes { get; set; }
    public DbSet<PaymentRequest> PaymentRequests { get; set; }
    public DbSet<CompanyIPG> CompanyIPGs { get; set; }
    public DbSet<CompanyIPGDeposit> CompanyIPGDeposits { get; set; }
    public DbSet<CompanyShaparakSetting> ShaparakSettings { get; set; }
    public DbSet<DirectDebitGrant> DirectDebitGrants { get; set; }
    public DbSet<DirectDebitPlan> DirectDebitPlans { get; set; }
    public DbSet<PaymentReceiptTransaction> PaymentReceiptTransactions { get; set; }
    public DbSet<CharismaCardTransaction> CharismaCardTransactions { get; set; }
    public DbSet<CompanyDepositPaymentMethod> CompanyDepositPaymentMethods { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .ApplyConfiguration(new ApplicationSettingsConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new UserRoleConfiguration())
            .ApplyConfiguration(new CompanyConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodConfiguration())
            .ApplyConfiguration(new BankConfiguration())
            .ApplyConfiguration(new ProviderConfiguration())
            .ApplyConfiguration(new CompanyDepositConfiguration())
            .ApplyConfiguration(new IPGTypeConfiguration())
            .ApplyConfiguration(new CompanyIPGConfiguration())
            .ApplyConfiguration(new CompanyIPGDepositConfiguration())
            .ApplyConfiguration(new ApplicationConfiguration())
            .ApplyConfiguration(new ApplicationIdentifierConfiguration())
            .ApplyConfiguration(new PaymentRequestConfiguration())
            .ApplyConfiguration(new ApplicationCallbackUrlConfiguration())
            .ApplyConfiguration(new TransactionConfiguration())
            .ApplyConfiguration(new IPGTransactionConfiguration())
            .ApplyConfiguration(new CompanyShaparakSettingConfiguration())
            .ApplyConfiguration(new BankDirectDebitSettingConfiguration())
            .ApplyConfiguration(new DirectDebitGrantConfiguration())
            .ApplyConfiguration(new DirectDebitPlanConfiguration())
            .ApplyConfiguration(new DirectDebitTransactionConfiguration())
            .ApplyConfiguration(new PaymentReceiptTransactionConfiguration())
            .ApplyConfiguration(new CharismaCardTransactionConfiguration())
            .ApplyConfiguration(new CompanyDepositPaymentMethodConfiguration())
        ;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        // Track entity changes before SaveChanges
        var addedCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
        var modifiedCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
        var deletedCount = ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted);

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            stopwatch.Stop();

            // Log successful SaveChanges operation
            if (addedCount > 0 || modifiedCount > 0 || deletedCount > 0)
            {
                _logService.LogDatabaseOperation(new DatabaseOperationLog
                {
                    OperationType = "SaveChanges",
                    ContextType = "WriteDbContext",
                    EntityCount = addedCount + modifiedCount + deletedCount,
                    RowsAffected = result,
                    IsSuccess = true,
                    IsSlow = stopwatch.ElapsedMilliseconds >= 1000,
                    StartDateTime = startTime,
                    EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    UserId = userId,
                    CompanyId = companyId,
                    ApplicationId = applicationId,
                    CorrelationId = correlationId,
                    RequestId = requestId
                });
            }

            await _mediator.DispatchDomainEventsAsync(this);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failed SaveChanges operation
            _logService.LogDatabaseOperation(new DatabaseOperationLog
            {
                OperationType = "SaveChanges",
                ContextType = "WriteDbContext",
                EntityCount = addedCount + modifiedCount + deletedCount,
                IsSuccess = false,
                IsSlow = false,
                ErrorCode = ex.GetType().Name,
                ErrorMessage = ex.Message,
                StartDateTime = startTime,
                EndDateTime = startTime.AddMilliseconds(stopwatch.ElapsedMilliseconds),
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId,
                CompanyId = companyId,
                ApplicationId = applicationId,
                CorrelationId = correlationId,
                RequestId = requestId
            });

            _logger.LogError(ex, "WriteDbContext SaveChanges failed with {AddedCount} added, {ModifiedCount} modified, {DeletedCount} deleted entities", addedCount, modifiedCount, deletedCount);
            throw;
        }
    }
}
