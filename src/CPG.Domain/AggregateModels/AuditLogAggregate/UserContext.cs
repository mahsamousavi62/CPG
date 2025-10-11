namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents user and client context information for audit logging
/// </summary>
public sealed record UserContext
{
    /// <summary>
    /// User identifier
    /// </summary>
    public long? UserId { get; init; }

    /// <summary>
    /// Client IP address
    /// </summary>
    public string? Ip { get; init; }

    /// <summary>
    /// User agent string from HTTP headers
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// Application/client identifier
    /// </summary>
    public long? ApplicationId { get; init; }

    /// <summary>
    /// Company identifier associated with the request
    /// </summary>
    public long? CompanyId { get; init; }

    /// <summary>
    /// Client ID (OAuth/IDP client identifier)
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Mobile phone number (if applicable)
    /// </summary>
    public string? MobilePhone { get; init; }

    private UserContext()
    {
    }

    public UserContext(
        long? userId = null,
        string? ip = null,
        string? userAgent = null,
        long? applicationId = null,
        long? companyId = null,
        string? clientId = null,
        string? mobilePhone = null)
    {
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        ApplicationId = applicationId;
        CompanyId = companyId;
        ClientId = clientId;
        MobilePhone = mobilePhone;
    }

    /// <summary>
    /// Creates a user context for anonymous requests
    /// </summary>
    public static UserContext Anonymous(string? ip = null, string? userAgent = null)
    {
        return new UserContext(ip: ip, userAgent: userAgent);
    }

    /// <summary>
    /// Creates a user context for authenticated user requests
    /// </summary>
    public static UserContext ForUser(
        long userId,
        string? ip = null,
        string? userAgent = null,
        long? applicationId = null,
        long? companyId = null)
    {
        return new UserContext(
            userId: userId,
            ip: ip,
            userAgent: userAgent,
            applicationId: applicationId,
            companyId: companyId);
    }

    /// <summary>
    /// Creates a user context for application/client requests
    /// </summary>
    public static UserContext ForClient(
        long applicationId,
        string? clientId = null,
        string? ip = null,
        long? companyId = null)
    {
        return new UserContext(
            applicationId: applicationId,
            clientId: clientId,
            ip: ip,
            companyId: companyId);
    }
}
