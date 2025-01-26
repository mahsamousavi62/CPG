using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Providers.Ipg;
using Microsoft.Extensions.Logging;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Infrastructure.Persistence.Redis;
namespace CPG.Infrastructure.Providers.DirectDebit;

public class DirectDebitFactory(IHttpProvider httpProvider,
        ILogService logService,
        IApplicationSettingsRepository applicationSettingsRepository,
        ReadDbContext context,
        ILogger<PecProvider> pecProviderLogger,
        IRedisCacheService redisCacheService) : IDirectDebitFactory
{
    private readonly IHttpProvider _httpProvider = httpProvider;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly ReadDbContext _context = context;
    private readonly ILogService _logService = logService;
    private readonly ILogger<PecProvider> _pecProviderLogger = pecProviderLogger;
    private readonly IRedisCacheService _redisCacheService = redisCacheService;

    public IDirectDebitProvider GetInstance(Enums.ProviderType providerType)
    {
        switch (providerType)
        {
            case Enums.ProviderType.Vandar:
                {
                    return new VandarProvider(_httpProvider, _context, _applicationSettingsRepository, _redisCacheService);
                }
            default: return null;
        }
    }
}
