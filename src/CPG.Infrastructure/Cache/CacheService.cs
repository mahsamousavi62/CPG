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
            var callLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = null,
                LogId = $"{Guid.NewGuid()} - Cache - {ex.GetType().Name}",
                RequestId = null,
                AuditLevel = 3, // Error
                AuditType = Enums.AuditType.Develop,
                ServiceName = "CacheService",
                ProviderName = "Redis",
                RequestUri = nameof(GetAllApplicationSettings),
                RequestHeader = null,
                RequestBody = "",
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = ex.Message,
                ApplicationId = null,
                UserId = null,
                Ip = null,
                CompanyId = null,
                UserAgent = null,
                Response = ex.Message,
                ErrorCode = ex.GetType().Name,
                IsSucceeded = false,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now,
                DurationMs = 0,
                StackTrace = ex.StackTrace,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = nameof(GetAllApplicationSettings),
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.Cache,
                CreationDate = DateTime.Now,
                CreationUserId = 1,
                ErrorType = ex.Message,
                ProviderType = Enums.ProviderTypeInLog.Internal,
                CorrolationId = null
            };
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
            var callLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = null,
                LogId = $"{Guid.NewGuid()} - Cache - {ex.GetType().Name}",
                RequestId = null,
                AuditLevel = 3, // Error
                AuditType = Enums.AuditType.Develop,
                ServiceName = "CacheService",
                ProviderName = "Redis",
                RequestUri = nameof(GetApplicationSettings),
                RequestHeader = null,
                RequestBody = "",
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = ex.Message,
                ApplicationId = null,
                UserId = null,
                Ip = null,
                CompanyId = null,
                UserAgent = null,
                Response = ex.Message,
                ErrorCode = ex.GetType().Name,
                IsSucceeded = false,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now,
                DurationMs = 0,
                StackTrace = ex.StackTrace,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = nameof(GetApplicationSettings),
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.Cache,
                CreationDate = DateTime.Now,
                CreationUserId = 1,
                ErrorType = ex.Message,
                ProviderType = Enums.ProviderTypeInLog.Internal,
                CorrolationId = null
            };
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
            var callLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = null,
                LogId = $"{Guid.NewGuid()} - Cache - {ex.GetType().Name}",
                RequestId = null,
                AuditLevel = 3, // Error
                AuditType = Enums.AuditType.Develop,
                ServiceName = "CacheService",
                ProviderName = "Redis",
                RequestUri = nameof(GetApplicationIdentifier),
                RequestHeader = null,
                RequestBody = "",
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = ex.Message,
                ApplicationId = null,
                UserId = null,
                Ip = null,
                CompanyId = null,
                UserAgent = null,
                Response = ex.Message,
                ErrorCode = ex.GetType().Name,
                IsSucceeded = false,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now,
                DurationMs = 0,
                StackTrace = ex.StackTrace,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = nameof(GetApplicationIdentifier),
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.Cache,
                CreationDate = DateTime.Now,
                CreationUserId = 1,
                ErrorType = ex.Message,
                ProviderType = Enums.ProviderTypeInLog.Internal,
                CorrolationId = null
            };
            logService.LogError(callLog);
            throw;
        }
    }
}

