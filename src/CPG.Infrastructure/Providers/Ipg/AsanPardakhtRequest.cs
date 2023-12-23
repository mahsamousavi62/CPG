using System;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class AsanPardakhtRequest
    {
        public int merchantConfigurationId { get; set; }
        public int serviceTypeId { get; set; }
        public string localInvoiceId { get; set; }
        public long amountInRials { get; set; }
        public string localDate => DateTime.Now.ToString("yyyyMMdd HHmmss");
        public string additionalData { get; set; }
        public string callbackURL { get; set; }
        public string paymentId { get; set; }
    }
}
