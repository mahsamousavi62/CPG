using System.Net.Http;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Logging;

public interface ILogService
{
    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderTypeInLog ProviderTypeInLog { get; set; }
    void AddServiceCallLog(string request, string response, short status, string message);
    Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response);
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response,string resString);

    /// <summary>
    /// Log timeout with request body for troubleshooting
    /// </summary>
    void AddTimeoutLog<TBody>(HttpProviderRequest<TBody> request, System.Exception exception, long durationMs);

    /// <summary>
    /// Log SOAP call with strongly-typed request/response objects for better serialization
    /// Serializes SOAP objects using Newtonsoft.Json and applies sensitive data masking
    /// </summary>
    /// <typeparam name="TRequest">SOAP request type</typeparam>
    /// <typeparam name="TResponse">SOAP response type</typeparam>
    /// <param name="request">SOAP request object</param>
    /// <param name="response">SOAP response object</param>
    /// <param name="serviceName">Service operation name</param>
    /// <param name="status">Status code (0 = success, negative = error)</param>
    /// <param name="message">Optional message or error description</param>
    void AddSoapCallLog<TRequest, TResponse>(TRequest request, TResponse response, string serviceName, short status, string message);

    /// <summary>
    /// Log SOAP timeout with request body capture for troubleshooting
    /// Used by Polly timeout policies to log complete SOAP request context
    /// </summary>
    /// <typeparam name="TRequest">SOAP request type</typeparam>
    /// <param name="request">SOAP request object to be serialized</param>
    /// <param name="serviceName">Service operation name</param>
    /// <param name="exception">Timeout exception</param>
    /// <param name="durationMs">Timeout duration in milliseconds</param>
    void AddSoapTimeoutLog<TRequest>(TRequest request, string serviceName, System.Exception exception, long durationMs);
}
