using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.Charispay.Models;

public class CharispayResponseBase<T>: ResponseBase
{
    public new List<T> result { get; set; }
    public object targetUrl { get; set; }
    public bool success { get; set; }
    public object error { get; set; }
    public bool unAuthorizedRequest { get; set; }
    public bool __abp { get; set; }
}

public class ResponseBase
{
    public string ErrorResult { get; set; }
   
}