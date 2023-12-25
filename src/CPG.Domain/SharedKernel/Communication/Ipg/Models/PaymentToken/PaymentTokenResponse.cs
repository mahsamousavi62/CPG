using System;
using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class Params
{
    public string Token { get; set; }
}

public class PaymentTokenResponse
{
    public string Url { get; set; }
    public string Method => "Post";
    public string TrackerId { get; set; }
    public Enums.IpgRedirectionMethodType  IpgRedirectionMethodType { get; set; }
    public UrlResponseModel JsonBody { get; set; }
}

public class UrlResponseModel
{
    public string Url { get; set; }
    public string Method => "Post";
    public Params Params { get; set; }
}
