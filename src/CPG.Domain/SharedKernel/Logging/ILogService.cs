using System;
using System.Net.Http;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Logging;

public interface ILogService
{
    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderTypeInLog ProviderTypeInLog { get; set; }

    // Service call logging methods (Legacy - use new methods instead)
    void AddServiceCallLog(string request, string response, short status, string message);
    Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response) where TBody : class where TRequest : class;
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString) where TBody : class;

    // New structured logging methods - accept CallLogModel directly
    void LogInformation(CallLogModel callLog);
    void LogWarning(CallLogModel callLog);
    void LogError(CallLogModel callLog);
    void LogDebug(CallLogModel callLog);
}
