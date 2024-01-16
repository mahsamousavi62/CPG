using System;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class AsanPardakhtTokenRequest : AsanPardakhtRequestBase
{
    [JsonProperty(PropertyName = "merchantConfigurationId")]
    public int MerchantConfigurationId { get; set; }
   
    [JsonProperty(PropertyName = "serviceTypeId")]
    public int ServiceTypeId { get; set; }
    
    [JsonProperty(PropertyName = "localInvoiceId")]
    public string LocalInvoiceId { get; set; }
    
    [JsonProperty(PropertyName = "amountInRials")]
    public long AmountInRials { get; set; }
    
    [JsonProperty(PropertyName = "localDate")]
    public string LocalDate => DateTime.Now.ToString("yyyyMMdd HHmmss", System.Globalization.CultureInfo.InvariantCulture);
    
    [JsonProperty(PropertyName = "additionalData")]
    public string AdditionalData { get; set; }
    
    [JsonProperty(PropertyName = "callbackURL")]
    public string CallbackURL { get; set; }
    
    [JsonProperty(PropertyName = "paymentId")]
    public string PaymentId { get; set; }
}
