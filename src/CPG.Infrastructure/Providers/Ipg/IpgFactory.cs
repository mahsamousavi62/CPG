
using System.Net.Http;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Infrastructure.Persistence.DbContexts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class IpgFactory(IHttpProvider httpProvider, 
        IMediator mediator,
        IApplicationSettingsRepository applicationSettingsRepository, 
        ReadDbContext context) : IIpgFactory
    {
        private readonly IHttpProvider _httpProvider = httpProvider;
        private readonly IMediator _mediator = mediator;
        private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
        private readonly ReadDbContext _context = context;

        public IIpgProvider GetInstance(Enums.ProviderType providerType)
        {
           switch (providerType) 
            {
                case Enums.ProviderType.AsanPardakht:
                    {
                        var asanpardakht= new AsanPardakhtProvider(_httpProvider, _context);
                        
                        asanpardakht.ApplicationSettingRepositoy = _applicationSettingsRepository;
                        return asanpardakht;
                    }
                    default: return null;
            }
        }
    }
}
