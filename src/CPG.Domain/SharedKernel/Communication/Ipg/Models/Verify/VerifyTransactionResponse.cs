using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class VerifyTransactionResponse : AsanPardakhtResponseBase
{
    public Enums.IPGTransactionStatus Status { get; set; }
}
