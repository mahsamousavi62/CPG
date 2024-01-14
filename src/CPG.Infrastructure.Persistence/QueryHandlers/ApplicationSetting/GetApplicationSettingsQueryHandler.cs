using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.ApplicationSetting
{
    public class GetApplicationSettingsQueryHandler : IRequestHandler<GetApplicationSettingsQuery, Result<IReadOnlyCollection<ApplicationSettingViewModel>>>
    {
        private readonly ReadDbContext _context;
        private readonly IRedisCaheService _cacheService;
        public const string CacheKey = "AllApplicationSetiings_key";
        public GetApplicationSettingsQueryHandler(ReadDbContext context, IRedisCaheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Result<IReadOnlyCollection<ApplicationSettingViewModel>>> Handle(GetApplicationSettingsQuery query, CancellationToken cancellationToken)
        {

            var applicationSettings = (await GetAll()).Where(x => x.EntityType == query.EntityType).ToList();
            return Result<IReadOnlyCollection<ApplicationSettingViewModel>>.SuccessResult(applicationSettings);
        }


        public async Task<IReadOnlyCollection<ApplicationSettingViewModel>> GetAll()
        {
            var cacheData = _cacheService.GetData<IReadOnlyCollection<ApplicationSettingViewModel>>(CacheKey);

            if (cacheData != null)
                return cacheData;

            cacheData = await _context.ApplicationSettingReadModels
                     .Select(x => new ApplicationSettingViewModel
                     {
                         Id = x.Id,
                         EntityType = x.EntityType,
                         EntityTypeName = ((Enums.ApplicationSettingEntityType)x.EntityType).ToString(),
                         Key = x.Key,
                         Value = x.Value,
                     })
                .ToListAsync();
            _cacheService.SetData<IReadOnlyCollection<ApplicationSettingViewModel>>(CacheKey, cacheData);
            return cacheData;
        }
    }
}
