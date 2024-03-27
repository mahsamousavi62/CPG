using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.DbContexts;
using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class IpgFactory(IHttpProvider httpProvider,
        ILogService logService,
        IApplicationSettingsRepository applicationSettingsRepository,
        ReadDbContext context,
         ILogger<PecProvider> pecProviderLogger) : IIpgFactory
    {
        private readonly IHttpProvider _httpProvider = httpProvider;
        private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
        private readonly ReadDbContext _context = context;
        private readonly ILogService _logService = logService;
        private readonly ILogger<PecProvider> _pecProviderLogger = pecProviderLogger;
        public IIpgProvider GetInstance(Enums.ProviderType providerType)
        {
            switch (providerType)
            {
                case Enums.ProviderType.AsanPardakht:
                    {
                        return new AsanPardakhtProvider(_httpProvider, _context, _applicationSettingsRepository);
                    }
                case Enums.ProviderType.Sep:
                    {
                        return new SepProvider(_httpProvider, _context, _applicationSettingsRepository);
                    }
                case Enums.ProviderType.Pec:
                    {
                        return new PecProvider(_context, _applicationSettingsRepository, _logService, _pecProviderLogger);
                    }
                case Enums.ProviderType.BehPardakht:
                    {
                        return new BehPardakhtProvider(_context, _applicationSettingsRepository, _logService, _pecProviderLogger);
                    }
                default: return null;
            }
        }
    }
}