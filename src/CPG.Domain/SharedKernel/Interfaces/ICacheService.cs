using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface ICacheService
{
    T? GetCache<T>(string key);
  
    void SetCache<T>(string key, T value, TimeSpan? slidingExpiration = null);
  
    void ClearCache(string key);

    Task ClearCacheAsync();
    
    Task<List<ApplicationIdentifier>> GetApplicationIdentifier();
  
    Task<List<ApplicationSettings>> GetApplicationSettings(CancellationToken cancellationToken = default);
}
