namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class StoreResponse : ResponseBase
{
    public string TrackerId { get; set; }

    public string Token { get; set; }

    public short Status { get; set; }

    public string Message { get; set; }
}
