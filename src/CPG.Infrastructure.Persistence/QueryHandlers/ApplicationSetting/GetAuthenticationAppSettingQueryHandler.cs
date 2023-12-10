using CPG.Application.UseCases.Common.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.ApplicationSetting;

public class GetAuthenticationAppSettingQueryHandler(ReadDbContext context, IRedisCaheService cacheService) 
    : IRequestHandler<GetAuthenticationAppSettingQuery, ApplicationConfigViewModel>
{
    private readonly ReadDbContext _context = context;
    private readonly IRedisCaheService _cacheService = cacheService;
    public const string CacheKey = "AuthenticationConfigApplicationSettings_key";

    public async Task<ApplicationConfigViewModel> Handle(GetAuthenticationAppSettingQuery query, CancellationToken cancellationToken)
    => await GetAll(query.EntityType);


    public async Task<ApplicationConfigViewModel> GetAll(Enums.ApplicationSettingEntityType entityType)
    {
        var cacheData = _cacheService.GetData<ApplicationConfigViewModel>(CacheKey);

        if (cacheData != null)
            return cacheData;

        var appSettings = await _context.ApplicationSettingReadModels
            .ToDictionaryAsync(t => t.Key, t => t.Value);


        cacheData = new ApplicationConfigViewModel();
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


