using System;

namespace CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

public class AsanPardakhtTokenRequest : AsanPardakhtRequestBase
{
    public int merchantConfigurationId { get; set; }
    public int serviceTypeId { get; set; }
    public string localInvoiceId { get; set; }
    public long amountInRials { get; set; }
    public string localDate => DateTime.Now.ToString("yyyyMMdd HHmmss", System.Globalization.CultureInfo.InvariantCulture);
    public string additionalData { get; set; }
    public string callbackURL { get; set; }
    public string paymentId { get; set; }
}
