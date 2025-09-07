using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard.Models;

//public class CharismaCardUserDepositBalanceResponse
//{
//    [JsonPropertyName("isSuccess")]
//    public bool IsSuccess { get; set; }

//    [JsonPropertyName("isFailure")]
//    public bool IsFailure { get; set; }

//    [JsonPropertyName("data")]
//    public List<CharismaCardData> Data { get; set; }

//    [JsonPropertyName("error")]
//    public CharismaCardError Error { get; set; }
//}

public class CharismaCardUserDepositBalanceResponse : CharismaCardBaseResponse<List<CharismaCardData>> { }

public class CharismaCardData
{
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }

    [JsonPropertyName("customerFirstName")]
    public string CustomerFirstName { get; set; }

    [JsonPropertyName("customerLastName")]
    public string CustomerLastName { get; set; }

    [JsonPropertyName("iban")]
    public string Iban { get; set; }

    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; }

    [JsonPropertyName("depositNumber")]
    public string DepositNumber { get; set; }

    [JsonPropertyName("urlAliasName")]
    public string UrlAliasName { get; set; }
}

