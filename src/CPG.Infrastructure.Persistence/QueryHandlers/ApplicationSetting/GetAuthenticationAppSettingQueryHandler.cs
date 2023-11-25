using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Authorization;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CPG.Infrastructure.Persistence.QueryHandlers.ApplicationSetting
{
    public class GetAuthenticationAppSettingQueryHandler : IRequestHandler<GetAuthenticationAppSettingQuery, AuthenticationConfigViewModel>
    {
        private readonly ReadDbContext _context;
        private readonly IRedisCaheService _cacheService;
        public const string CacheKey = "AuthenticationConfigApplicationSettings_key";
        public GetAuthenticationAppSettingQueryHandler(ReadDbContext context, IRedisCaheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }
        public async Task<AuthenticationConfigViewModel> Handle(GetAuthenticationAppSettingQuery query, CancellationToken cancellationToken)
        => await GetAll(query.EntityType);


        public async Task<AuthenticationConfigViewModel> GetAll(Enums.ApplicationSettingEntityType entityType)
        {
            var cacheData = _cacheService.GetData<AuthenticationConfigViewModel>(CacheKey);

            if (cacheData != null)
                return cacheData;

            var appSettings = await _context.ApplicationSettingReadModels
                .Where(x => x.EntityType == entityType)
                .ToDictionaryAsync(t => t.Key, t => t.Value);

            cacheData = new AuthenticationConfigViewModel();
            var type = cacheData.GetType();
            var prs = type.GetProperties();
            foreach (var item in prs)

            {
                item.SetValue(cacheData, Convert.ChangeType(appSettings[item.Name], Type.GetTypeCode(item.PropertyType)));
            }

            _cacheService.SetData(CacheKey, cacheData);

            return cacheData;
        }
    }

    
}
