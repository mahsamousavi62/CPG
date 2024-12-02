using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Interfaces;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Cache;

public class CacheService(IDistributedCache cache,
                          IAggregateRepository<ApplicationSettings> applicationSettingsRepository,
                          IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationReadModelRepository,
                          ILogger<CacheService> logger) : ICacheService
{
    private const string ApplicationIdentifierKey = nameof(ApplicationIdentifier);


    private const string AllApplicationSettingsKey = "AllApplicationSettings";

    public T? GetCache<T>(string key)
    {
        string? jsonData = cache.GetString(key);
        if (jsonData == null)
            return default;
        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public void SetCache<T>(string key, T value, TimeSpan? slidingExpirationTime = null)
    {
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = slidingExpirationTime
        };
        string jsonData = JsonSerializer.Serialize(value);
        cache.SetString(key, jsonData, options);
    }

    public void ClearCache(string key)
    {
        cache.Remove(key);
    }

    public async Task ClearCacheAsync()
    {
        await cache.RemoveAsync("AllApplicationSettings");
    }

    public async Task<List<ApplicationSettings>> GetAllApplicationSettings(CancellationToken cancellationToken)
    {
        try
        {
            List<ApplicationSettings>? cacheData = GetCache<List<ApplicationSettings>>(AllApplicationSettingsKey);

            if (cacheData != null)
                return cacheData;

            List<ApplicationSettings> applicationSettings = await applicationSettingsRepository.ListAsync(cancellationToken);

            SetCache(AllApplicationSettingsKey, applicationSettings, TimeSpan.FromHours(1));

            return cacheData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(GetAllApplicationSettings));
            throw;
        }
    }

    public async Task<List<ApplicationSettings>> GetApplicationSettings(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetAllApplicationSettings(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(GetApplicationSettings));
            throw;
        }
    }

    public async Task<List<ApplicationIdentifier>> GetApplicationIdentifier()
    {
        try
        {
            List<ApplicationIdentifier> cacheData = GetCache<List<ApplicationIdentifier>>(ApplicationIdentifierKey);

            if (cacheData != null)
                return cacheData;

            var applications = await applicationReadModelRepository.ListAsync();

            cacheData = applications.SelectMany(t => t.ApplicationIdentifiers).Adapt<List<ApplicationIdentifier>>();

            SetCache(ApplicationIdentifierKey, cacheData);

            return cacheData!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
}

