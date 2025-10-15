using System.Net.Http;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Logging;

public interface ILogService
{
    // ===== Legacy Properties (kept for backward compatibility) =====
    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderTypeInLog ProviderTypeInLog { get; set; }

    // ===== Primary Method - Use This for All New Code =====

    /// <summary>
    /// Main logging method - Logs a service call with complete CallLogModel
    /// This is the primary method that should be used for all new code.
    /// Supports all 25 parameters from task.md specification.
    /// </summary>
    /// <param name="logModel">Complete call log model with all required fields</param>
    void LogServiceCall(CallLogModel logModel);

    // ===== Legacy Methods (kept for backward compatibility) =====
    // These methods internally map to LogServiceCall

    /// <summary>
    /// Legacy method - Use LogServiceCall instead
    /// </summary>
    void AddServiceCallLog(string request, string response, short status, string message);

    /// <summary>
    /// Legacy method - Use LogServiceCall instead
    /// </summary>
    Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response);

    /// <summary>
    /// Legacy method - Use LogServiceCall instead
    /// </summary>
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString);

    /// <summary>
    /// Log timeout with request body for troubleshooting
    /// Legacy method - Use LogServiceCall instead
    /// </summary>
    void AddTimeoutLog<TBody>(HttpProviderRequest<TBody> request, System.Exception exception, long durationMs);

    /// <summary>
    /// Log SOAP call with strongly-typed request/response objects for better serialization
    /// Serializes SOAP objects using Newtonsoft.Json and applies sensitive data masking
    /// Legacy method - Use LogServiceCall instead
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
    /// Legacy method - Use LogServiceCall instead
    /// </summary>
    /// <typeparam name="TRequest">SOAP request type</typeparam>
    /// <param name="request">SOAP request object to be serialized</param>
    /// <param name="serviceName">Service operation name</param>
    /// <param name="exception">Timeout exception</param>
    /// <param name="durationMs">Timeout duration in milliseconds</param>
    void AddSoapTimeoutLog<TRequest>(TRequest request, string serviceName, System.Exception exception, long durationMs);

    // ===== Specialized Logging Methods (from IAuditLogService) =====
    // These are helper methods that convert specific log types to CallLogModel

    /// <summary>
    /// Log MinIO operations (file storage operations)
    /// Maps MinioOperationLog to CallLogModel
    /// </summary>
    void LogMinioOperation(MinioOperationLog log);

    /// <summary>
    /// Log database operations (EF Core query and command operations)
    /// Maps DatabaseOperationLog to CallLogModel
    /// </summary>
    void LogDatabaseOperation(DatabaseOperationLog log);

    /// <summary>
    /// Log provider call operations (HTTP/SOAP)
    /// Maps ProviderCallLog to CallLogModel
    /// </summary>
    void LogProviderCall(ProviderCallLog log);
}
