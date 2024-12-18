using System;

namespace CPG.Infrastructure.Authorization.Models;

public class ExternalServiceCallLog
{
    public string? RequestBody { get; set; }

    public int ServiceCallStatusCode { get; set; }

    public string? ServiceCallUrl { get; set; }

    public DateTime ServiceCallDate { get; set; }

    public string? ResponseBody { get; set; }

    public string? ClientId { get; set; }
}
