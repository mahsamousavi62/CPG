namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class VerifyTransactionResponse : ResponseBase
{
    public Enums.IPGTransactionStatus Status { get; set; }

    public string RRN { get; set; }
}
