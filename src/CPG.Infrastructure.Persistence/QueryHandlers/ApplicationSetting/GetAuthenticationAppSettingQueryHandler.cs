using CPG.Application.Auth;
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

public class GetAuthenticationAppSettingQueryHandler(ReadDbContext context, IRedisCacheService cacheService, IAuthService authService) 
    : IRequestHandler<GetAuthenticationAppSettingQuery, JwtConfigViewModel>
{
    private readonly ReadDbContext _context = context;
    private readonly IRedisCacheService _cacheService = cacheService;
    private readonly IAuthService _authService = authService;
    public const string CacheKey = "AuthenticationConfigApplicationSettings_key";

    public async Task<JwtConfigViewModel> Handle(GetAuthenticationAppSettingQuery query, CancellationToken cancellationToken)
    => await GetAll(query.EntityType);


    public async Task<JwtConfigViewModel> GetAll(Enums.ApplicationSettingEntityType entityType)
    {
        var cacheData = _cacheService.GetData<JwtConfigViewModel>(CacheKey);

        if (cacheData != null)
            return cacheData;

        cacheData = _authService.GetJwtConfig();

        //cacheData = new JwtConfigViewModel();
        //var type = cacheData.GetType();
        //var prs = type.GetProperties();
        //foreach (var item in prs)
        //{
        //    item.SetValue(cacheData, Convert.ChangeType(jwtConfig[item.Name], Type.GetTypeCode(item.PropertyType)));
        //}

        _cacheService.SetData(CacheKey, cacheData);

        return cacheData;
    }
}


