
using System.Net.Http;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Infrastructure.Persistence.DbContexts;
using MassTransit;
using MediatR;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class IpgFactory(IHttpClientFactory factory, 
        IMediator mediator,IApplicationSettingsRepository applicationSettingsRepository, ReadDbContext context) : IIpgFactory
    {
        private readonly IHttpClientFactory _factory = factory;
        private readonly IMediator _mediator = mediator;
        private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
        private readonly ReadDbContext _context = context;

        public IIpgProvider GetInstance(Enums.ProviderType providerType)
        {
           switch (providerType) 
            {
                case Enums.ProviderType.AsanPardakht:
                    {
                        var asanpardakht= new AsanPardakhtProvider(factory, null);
                        asanpardakht.ClientFactory= _factory;
                        asanpardakht.Mediator=_mediator;
                        asanpardakht.ApplicationSettingRepositoy = _applicationSettingsRepository;
                        asanpardakht.Context=_context;
                        return asanpardakht;
                    }
                    default: return null;
            }
        }
    }
}
