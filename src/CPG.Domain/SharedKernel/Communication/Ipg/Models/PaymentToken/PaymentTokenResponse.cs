using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class Params
{
    [JsonPropertyName("RefID")]
    public string RefID { get; set; }
}

public class PaymentTokenResponse
{
    public string Url { get; set; }
    public string Method => "Post";
    public string TrackerId { get; set; }
    public Enums.IpgRedirectionMethodType  IpgRedirectionMethodType { get; set; }
    public JsonStrModel JsonBody { get; set; }
}
public class JsonStrModel
{
    public UrlResponseModel JsonStr { get; set; }
}
public class UrlResponseModel
{
    public string Url { get; set; }
    public string Method => "Post";
    public Params Params { get; set; }
}
