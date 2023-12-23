using Newtonsoft.Json;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Communication;

public class RequestBase
{
    [JsonIgnore]
    public ProviderType Provider { get; set; }
}
