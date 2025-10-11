using System;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;
using static CPG.Domain.SharedKernel.EnumExtensions;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents service and provider context information for audit logging
/// </summary>
public sealed record ServiceContext
{
    /// <summary>
    /// Name of the service being called
    /// </summary>
    public string ServiceName { get; init; }

    /// <summary>
    /// Type of service (IPG, DirectDebit, etc.)
    /// </summary>
    public ServiceType? ServiceType { get; init; }

    /// <summary>
    /// Provider name (Vandar, AsanPardakht, Sep, etc.)
    /// </summary>
    public string? ProviderName { get; init; }

    /// <summary>
    /// Provider type for logging purposes
    /// </summary>
    public ProviderTypeInLog? ProviderType { get; init; }

    private ServiceContext()
    {
    }

    public ServiceContext(
        string serviceName,
        ServiceType? serviceType = null,
        string? providerName = null,
        ProviderTypeInLog? providerType = null)
    {
        ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        ServiceType = serviceType;
        ProviderName = providerName;
        ProviderType = providerType;
    }

    /// <summary>
    /// Creates a service context for internal CPG services
    /// </summary>
    public static ServiceContext ForInternalService(string serviceName, ServiceType? serviceType = null)
    {
        return new ServiceContext(serviceName, serviceType);
    }

    /// <summary>
    /// Creates a service context for external provider calls
    /// </summary>
    public static ServiceContext ForProvider(
        string serviceName,
        ProviderTypeInLog providerType,
        ServiceType? serviceType = null)
    {
        return new ServiceContext(
            serviceName,
            serviceType,
            providerType.GetName(),
            providerType);
    }
}
