using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Builder pattern for constructing AuditLog instances with a fluent API
/// </summary>
public sealed class AuditLogBuilder
{
    private LogId? _logId;
    private string? _correlationId;
    private AuditLevel _level = AuditLevel.Information;
    private string? _serviceName;
    private AuditType _auditType = AuditType.Client;
    private DateTime _startDateTime = DateTime.UtcNow;
    private DateTime? _endDateTime;
    private long? _durationMs;

    // Service context
    private ServiceType? _serviceType;
    private string? _providerName;
    private ProviderTypeInLog? _providerType;

    // User context
    private long? _userId;
    private string? _ip;
    private string? _userAgent;
    private long? _applicationId;
    private long? _companyId;
    private string? _clientId;

    // HTTP request/response
    private string? _requestUri;
    private string? _requestHeader;
    private string? _requestBody;
    private int? _responseStatusCode;
    private string? _responseHeader;
    private string? _responseBody;

    // Error info
    private string? _errorCode;
    private string? _errorType;

    public AuditLogBuilder()
    {
    }

    /// <summary>
    /// Sets the log ID
    /// </summary>
    public AuditLogBuilder WithLogId(LogId logId)
    {
        _logId = logId;
        return this;
    }

    /// <summary>
    /// Sets the log ID from components
    /// </summary>
    public AuditLogBuilder WithLogId(string id, string domain, string abbreviation)
    {
        _logId = new LogId(id, domain, abbreviation);
        return this;
    }

    /// <summary>
    /// Sets the correlation ID
    /// </summary>
    public AuditLogBuilder WithCorrelationId(string correlationId)
    {
        _correlationId = correlationId;
        return this;
    }

    /// <summary>
    /// Sets the audit level
    /// </summary>
    public AuditLogBuilder WithLevel(AuditLevel level)
    {
        _level = level;
        return this;
    }

    /// <summary>
    /// Sets the service name
    /// </summary>
    public AuditLogBuilder WithServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    /// <summary>
    /// Sets the audit type
    /// </summary>
    public AuditLogBuilder WithAuditType(AuditType auditType)
    {
        _auditType = auditType;
        return this;
    }

    /// <summary>
    /// Sets service context information
    /// </summary>
    public AuditLogBuilder WithServiceContext(ServiceContext serviceContext)
    {
        _serviceName = serviceContext.ServiceName;
        _serviceType = serviceContext.ServiceType;
        _providerName = serviceContext.ProviderName;
        _providerType = serviceContext.ProviderType;
        return this;
    }

    /// <summary>
    /// Sets user context information
    /// </summary>
    public AuditLogBuilder WithUserContext(UserContext userContext)
    {
        _userId = userContext.UserId;
        _ip = userContext.Ip;
        _userAgent = userContext.UserAgent;
        _applicationId = userContext.ApplicationId;
        _companyId = userContext.CompanyId;
        _clientId = userContext.ClientId;
        return this;
    }

    /// <summary>
    /// Sets HTTP request information
    /// </summary>
    public AuditLogBuilder WithRequest(HttpRequestLog request)
    {
        _requestUri = request.Uri;
        _requestHeader = request.Header;
        _requestBody = request.Body;
        return this;
    }

    /// <summary>
    /// Sets HTTP response information
    /// </summary>
    public AuditLogBuilder WithResponse(HttpResponseLog response)
    {
        _responseStatusCode = response.StatusCode;
        _responseHeader = response.Header;
        _responseBody = response.Body;
        return this;
    }

    /// <summary>
    /// Sets timing information
    /// </summary>
    public AuditLogBuilder WithTiming(TimingInfo timing)
    {
        _startDateTime = timing.StartDateTime;
        _endDateTime = timing.EndDateTime;
        _durationMs = timing.DurationMs;
        return this;
    }

    /// <summary>
    /// Sets error information
    /// </summary>
    public AuditLogBuilder WithError(ErrorInfo error)
    {
        _errorCode = error.ErrorCode;
        _errorType = error.ErrorType;
        _level = AuditLevel.Error;
        return this;
    }

    /// <summary>
    /// Sets start date time
    /// </summary>
    public AuditLogBuilder WithStartDateTime(DateTime startDateTime)
    {
        _startDateTime = startDateTime;
        return this;
    }

    /// <summary>
    /// Sets end date time (automatically calculates duration)
    /// </summary>
    public AuditLogBuilder WithEndDateTime(DateTime endDateTime)
    {
        _endDateTime = endDateTime;
        _durationMs = (long)(endDateTime - _startDateTime).TotalMilliseconds;
        return this;
    }

    /// <summary>
    /// Sets duration in milliseconds (automatically calculates end time)
    /// </summary>
    public AuditLogBuilder WithDuration(long durationMs)
    {
        _durationMs = durationMs;
        _endDateTime = _startDateTime.AddMilliseconds(durationMs);
        return this;
    }

    /// <summary>
    /// Sets user ID
    /// </summary>
    public AuditLogBuilder WithUserId(long userId)
    {
        _userId = userId;
        return this;
    }

    /// <summary>
    /// Sets company ID
    /// </summary>
    public AuditLogBuilder WithCompanyId(long companyId)
    {
        _companyId = companyId;
        return this;
    }

    /// <summary>
    /// Sets application ID
    /// </summary>
    public AuditLogBuilder WithApplicationId(long applicationId)
    {
        _applicationId = applicationId;
        return this;
    }

    /// <summary>
    /// Sets IP address
    /// </summary>
    public AuditLogBuilder WithIp(string ip)
    {
        _ip = ip;
        return this;
    }

    /// <summary>
    /// Sets user agent
    /// </summary>
    public AuditLogBuilder WithUserAgent(string userAgent)
    {
        _userAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Sets service type
    /// </summary>
    public AuditLogBuilder WithServiceType(ServiceType serviceType)
    {
        _serviceType = serviceType;
        return this;
    }

    /// <summary>
    /// Sets provider information
    /// </summary>
    public AuditLogBuilder WithProvider(ProviderTypeInLog providerType, string? providerName = null)
    {
        _providerType = providerType;
        _providerName = providerName ?? providerType.GetName();
        return this;
    }

    /// <summary>
    /// Marks the log as successful (Information level)
    /// </summary>
    public AuditLogBuilder AsSuccess()
    {
        _level = AuditLevel.Information;
        _errorCode = null;
        _errorType = null;
        return this;
    }

    /// <summary>
    /// Marks the log as warning
    /// </summary>
    public AuditLogBuilder AsWarning()
    {
        _level = AuditLevel.Warning;
        return this;
    }

    /// <summary>
    /// Marks the log as error with exception
    /// </summary>
    public AuditLogBuilder AsError(Exception exception, string? errorCode = null)
    {
        _level = AuditLevel.Error;
        _errorCode = errorCode ?? exception.GetType().Name;
        _errorType = exception.GetType().Name;
        return this;
    }

    /// <summary>
    /// Marks the log as error with custom error information
    /// </summary>
    public AuditLogBuilder AsError(string errorCode, string errorType)
    {
        _level = AuditLevel.Error;
        _errorCode = errorCode;
        _errorType = errorType;
        return this;
    }

    /// <summary>
    /// Completes the timing and builds the audit log
    /// </summary>
    public AuditLog Build()
    {
        // Validate required fields
        if (_logId == null)
            throw new InvalidOperationException("LogId is required");
        if (string.IsNullOrWhiteSpace(_correlationId))
            throw new InvalidOperationException("CorrelationId is required");
        if (string.IsNullOrWhiteSpace(_serviceName))
            throw new InvalidOperationException("ServiceName is required");

        // Calculate end time and duration if not set
        if (!_endDateTime.HasValue)
        {
            _endDateTime = DateTime.UtcNow;
        }
        if (!_durationMs.HasValue)
        {
            _durationMs = (long)(_endDateTime.Value - _startDateTime).TotalMilliseconds;
        }

        return new AuditLog(
            logId: _logId,
            correlationId: _correlationId,
            level: _level,
            serviceName: _serviceName,
            auditType: _auditType,
            startDateTime: _startDateTime,
            endDateTime: _endDateTime.Value,
            durationMs: _durationMs.Value,
            serviceType: _serviceType,
            providerName: _providerName,
            providerType: _providerType,
            userId: _userId,
            ip: _ip,
            userAgent: _userAgent,
            applicationId: _applicationId,
            companyId: _companyId,
            clientId: _clientId,
            requestUri: _requestUri,
            requestHeader: _requestHeader,
            requestBody: _requestBody,
            responseStatusCode: _responseStatusCode,
            responseHeader: _responseHeader,
            responseBody: _responseBody,
            errorCode: _errorCode,
            errorType: _errorType);
    }

    /// <summary>
    /// Creates a new builder instance
    /// </summary>
    public static AuditLogBuilder Create()
    {
        return new AuditLogBuilder();
    }
}
