using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;

public class TransactionResultRequest: AsanPardakhtRequestBase
{
    public string LocalInvoiceId { get; set; }

    public string ProviderData { get; set; }
}
