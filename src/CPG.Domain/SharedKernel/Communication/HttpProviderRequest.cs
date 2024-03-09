using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Communication;

public class HttpProviderRequest<T> : RequestBase
{
    public string BaseAddress { get; set; }
    public string Uri { get; set; }
    public IReadOnlyList<(string Key, string Value)> HeaderParameters { get; set; }
    public T? QueryParameters { get; set; }
    public T? Body { get; set; }
    public ServiceType Service { get; set; }
}


public sealed class HttpProviderRequest<TBody, TRequest>
{
    public required TRequest? Request { get; init; }

    public required string? Uri { get; set; }

    public string? BaseAddress { get; set; }

    public IReadOnlyList<(string Key, string Value)>? HeaderParameters { get; set; }

    public TBody? Body { get; set; }

    public bool ForceTls13 { get; set; }

    public ServiceType? Service { get; set; }

    public ProviderType? ProviderType { get; set; }
}
