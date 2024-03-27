namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class SettleTransactionResponse : ResponseBase
{
    public Enums.IPGTransactionStatus Status { get; set; }
}
