namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class StoreResponse : ResponseBase
{
    public string TrackerId { get; set; }

    public string Token { get; set; }

    public short GrantStatus { get; set; }

    public string GrantMessage { get; set; }
}
