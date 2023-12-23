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
