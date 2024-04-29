using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public IQueryable<ApplicationSettingReadModel> ApplicationSettingReadModels => Set<ApplicationSettingReadModel>().AsNoTracking();

    public IQueryable<CompanyReadModel> CompanyReadModels => Set<CompanyReadModel>().AsNoTracking();

    public IQueryable<BankReadModel> BankReadModels => Set<BankReadModel>().AsNoTracking();

    public IQueryable<CompanyDepositReadModel> CompanyDepositReadModels => Set<CompanyDepositReadModel>().AsNoTracking();

    public IQueryable<ProviderReadModel> ProviderReadModels => Set<ProviderReadModel>().AsNoTracking();

    public IQueryable<UserReadModel> UserReadModels => Set<UserReadModel>().AsNoTracking();

    public IQueryable<UserRoleReadModel> UserRoleReadModels => Set<UserRoleReadModel>().AsNoTracking();

    public IQueryable<ApplicationReadModel> ApplicationReadModels => Set<ApplicationReadModel>().AsNoTracking();

    public IQueryable<ApplicationIdentifierReadModel> ApplicationIdentifierReadModels => Set<ApplicationIdentifierReadModel>().AsNoTracking();

    public IQueryable<IPGTypeReadModel> IPGTypeReadModels => Set<IPGTypeReadModel>().AsNoTracking();

    public IQueryable<CompanyIPGReadModel> CompanyIPGReadModels => Set<CompanyIPGReadModel>().AsNoTracking();

    public IQueryable<CompanyIPGDepositReadModel> CompanyIPGDepositReadModels => Set<CompanyIPGDepositReadModel>().AsNoTracking();

    public IQueryable<ApplicationCallbackUrlReadModel> ApplicationCallbackUrlReadModels => Set<ApplicationCallbackUrlReadModel>().AsNoTracking();

    public IQueryable<PaymentRequestReadModel> PaymentRequestReadModels=> Set<PaymentRequestReadModel>().AsNoTracking();

    public IQueryable<IPGTransactionReadModel> IPGTransactionReadModels => Set<IPGTransactionReadModel>().AsNoTracking();
    
    public IQueryable<TransactionReadModel> TransactionReadModels => Set<TransactionReadModel>().AsNoTracking();

    public IQueryable<CompanyShaparakSettingReadModel> ShaparakSettingReadModels => Set<CompanyShaparakSettingReadModel>().AsNoTracking();

    public IQueryable<BankDirectDebitSettingReadModel> DirectDebitSettingReadModels => Set<BankDirectDebitSettingReadModel>().AsNoTracking();

    public IQueryable<DirectDebitGrantReadModel> DirectDebitGrantReadModels => Set<DirectDebitGrantReadModel>().AsNoTracking();

    public IQueryable<DirectDebitPlanReadModel> DirectDebitPlanReadModels => Set<DirectDebitPlanReadModel>().AsNoTracking();

    public IQueryable<DirectDebitTransactionReadModel> DebitTransactionReadModels => Set<DirectDebitTransactionReadModel>().AsNoTracking();

    public IQueryable<PaymentReceiptTransactionReadModel> PaymentReceiptTransactionReadModels => Set<PaymentReceiptTransactionReadModel>().AsNoTracking();

    public IQueryable<CharismaCardTransactionReadModel> CharismaCardTransactionReadModels => Set<CharismaCardTransactionReadModel>().AsNoTracking();

    public IQueryable<CompanyDepositPaymentMethodReadModel> CompanyDepositPaymentMethodReadModels => Set<CompanyDepositPaymentMethodReadModel>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ApplicationSettingReadModelConfiguration())
            .ApplyConfiguration(new CompanyReadModelConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodsReadModelConfiguration())
            .ApplyConfiguration(new BankReadModelConfiguration())
            .ApplyConfiguration(new CompanyDepositReadModelConfiguration())
            .ApplyConfiguration(new UserReadModelConfiguration())
            .ApplyConfiguration(new UserRoleReadModelConfiguration())
            .ApplyConfiguration(new ProviderReadModelConfiguration())
            .ApplyConfiguration(new ProviderPaymentMethodReadModelConfiguration())
            .ApplyConfiguration(new ApplicationReadModelConfiguration())
            .ApplyConfiguration(new ApplicationIdentifierReadModelConfiguration())
            .ApplyConfiguration(new ApplicationCallbackUrlReadModelConfiguration())
            .ApplyConfiguration(new IPGTypeReadModelConfiguration())
            .ApplyConfiguration(new PaymentRequestReadModelConfiguration())
            .ApplyConfiguration(new CompanyIPGReadModelConfiguration())
            .ApplyConfiguration(new CompanyIPGDepositReadModelConfiguration())
            .ApplyConfiguration(new IPGTransactionReadModelConfiguration())
            .ApplyConfiguration(new TransactionReadModelConfiguration())
            .ApplyConfiguration(new CompanyShaparakSettingReadModelConfiguration())
            .ApplyConfiguration(new BankDirectDebitSettingReadModelConfiguration())
            .ApplyConfiguration(new DirectDebitGrantReadModelConfiguration())
            .ApplyConfiguration(new DirectDebitPlanReadModelConfiguration())
            .ApplyConfiguration(new DirectDebitTransactionReadModelConfiguration())
            .ApplyConfiguration(new PaymentReceiptTransactionReadModelConfiguration())
            .ApplyConfiguration(new CharismaCardTransactionReadModelConfiguration())
            .ApplyConfiguration(new CompanyDepositPaymentMethodReadModelConfiguration())
            ;
    }

    public async Task<long> GetNextSequenceValue()
    {
        var p = new SqlParameter("@result", System.Data.SqlDbType.BigInt)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        await Database.ExecuteSqlRawAsync("set @result = NEXT VALUE FOR dbo.IncrementalCodeSequence", p);
        var nextVal = (long)p.Value;

        return nextVal;
    }
}