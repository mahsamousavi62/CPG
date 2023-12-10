using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly IRedisCaheService _cacheService;
        private readonly ReadDbContext _context;
       public const string CacheKey = "AuthenticationConfigApplicationSettings_key";
        public ApplicationSettingsRepository(ReadDbContext context, IRedisCaheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<ApplicationConfigViewModel> GetAllApplicationSettings()
        {
            var cacheData = _cacheService.GetData<ApplicationConfigViewModel>(CacheKey);

            if (cacheData != null)
                return cacheData;

            var appSettings = await _context.ApplicationSettingReadModels.ToDictionaryAsync(t => t.Key, t => t.Value);

            cacheData = new ApplicationConfigViewModel();
            var type = cacheData.GetType();
            var prs = type.GetProperties();
            try
            {
                foreach (var item in prs)
                {
                    item.SetValue(cacheData, Convert.ChangeType(appSettings[item.Name], Type.GetTypeCode(item.PropertyType)));
                }
            }
            catch (Exception)
            {

                
            }

            _cacheService.SetData(CacheKey, cacheData);

            return cacheData;
        }
    }
}
