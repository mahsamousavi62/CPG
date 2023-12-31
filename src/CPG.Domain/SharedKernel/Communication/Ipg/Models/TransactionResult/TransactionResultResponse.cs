using System.Text.Json.Serialization;
using System;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult
{
    public class TransactionResultResponse: AsanPardakhtResponseBase
    {
        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }

        [JsonPropertyName("rrn")]
        public string Rrn { get; set; }

        [JsonPropertyName("refID")]
        public string RefID { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("payGateTranID")]
        public string PayGateTranID { get; set; }

        [JsonPropertyName("salesOrderID")]
        public string SalesOrderID { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("serviceTypeId")]
        public int ServiceTypeId { get; set; }

        [JsonPropertyName("serviceStatusCode")]
        public object ServiceStatusCode { get; set; }

        [JsonPropertyName("destinationMobile")]
        public object DestinationMobile { get; set; }

        [JsonPropertyName("productId")]
        public object ProductId { get; set; }

        [JsonPropertyName("productNameFa")]
        public object ProductNameFa { get; set; }

        [JsonPropertyName("productPrice")]
        public object ProductPrice { get; set; }

        [JsonPropertyName("operatorId")]
        public object OperatorId { get; set; }

        [JsonPropertyName("operatorNameFa")]
        public object OperatorNameFa { get; set; }

        [JsonPropertyName("simTypeId")]
        public object SimTypeId { get; set; }

        [JsonPropertyName("simTypeTitleFa")]
        public object SimTypeTitleFa { get; set; }

        [JsonPropertyName("billId")]
        public object BillId { get; set; }

        [JsonPropertyName("payId")]
        public object PayId { get; set; }

        [JsonPropertyName("billOrganizationNameFa")]
        public object BillOrganizationNameFa { get; set; }

        [JsonPropertyName("payGateTranDate")]
        public DateTime PayGateTranDate { get; set; }

        [JsonPropertyName("payGateTranDateEpoch")]
        public double PayGateTranDateEpoch { get; set; }
    }
}
