namespace CPG.Domain.SharedKernel.Communication;

public class ResponseBase
{
    [Newtonsoft.Json.JsonIgnore]
    public short Status { get; set; }
}
