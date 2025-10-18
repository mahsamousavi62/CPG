using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Cache;

public class CacheService(IDistributedCache cache,
                          IAggregateRepository<ApplicationSettings> applicationSettingsRepository,
                          IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository,
                          ILogService logService) : ICacheService
{
    private const string ApplicationIdentifierKey = nameof(ApplicationIdentifier);

    private const string AllApplicationSettingsKey = "AllApplicationSettings";

    private readonly JsonSerializerSettings jsonSerializerOptions = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    public T? GetCache<T>(string key)
    {
        string? jsonData = cache.GetString(key);
        if (jsonData == null)
            return default;
        return JsonConvert.DeserializeObject<T>(jsonData, jsonSerializerOptions);
    }

    public void SetCache<T>(string key, T value, TimeSpan? slidingExpirationTime = null)
    {
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = slidingExpirationTime
        };

        string jsonData = JsonConvert.SerializeObject(value, jsonSerializerOptions);
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
            var callLog = CallLogModel.CreateError(
                serviceName: "CacheService",
                providerName: "Redis",
                requestUri: nameof(GetAllApplicationSettings),
                requestBody: "",
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.Cache,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                userId: 1
            );
            logService.LogError(callLog);
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
            var callLog = CallLogModel.CreateError(
                serviceName: "CacheService",
                providerName: "Redis",
                requestUri: nameof(GetApplicationSettings),
                requestBody: "",
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.Cache,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                userId: 1
            );
            logService.LogError(callLog);
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

            var applications = await applicationRepository.ListAsync(new ApplicationIncludeIdentifiersSpec());

            cacheData = applications.SelectMany(t => t.ApplicationIdentifiers).ToList();

            SetCache(ApplicationIdentifierKey, cacheData);

            return cacheData!;
        }
        catch (Exception ex)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: "CacheService",
                providerName: "Redis",
                requestUri: nameof(GetApplicationIdentifier),
                requestBody: "",
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.Cache,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                userId: 1
            );
            logService.LogError(callLog);
            throw;
        }
    }
}

