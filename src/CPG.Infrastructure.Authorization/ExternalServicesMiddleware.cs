using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using CPG.Domain.SharedKernel.Interfaces;
using System.Linq;
using CPG.Infrastructure.Authorization.Models;
using CPG.Domain.SharedKernel;
using CPG.Domain.AggregateModels.ApplicationAggregate;

namespace CPG.Infrastructure.Authorization;

public class ExternalServicesMiddleware(RequestDelegate next, ILogger<ExternalServicesMiddleware> logger)
{
    private readonly List<string> anonymousApis = new List<string> { "api/common", "graphql", "hangfire", "ipgresult", "GetPaymentMethods", "CancelPaymentRequest", "CreateIPGJsonStr", "CreatePaymentReceiptRequest", "GetDepositsByPaymentCode", "AnonymousStatus" };

    public async Task Invoke([NotNull] HttpContext httpContext, [NotNull] ICacheService cacheService, IConfiguration configuration)
    {
        ScopeModel Scope = configuration.GetSection("Scope").Get<ScopeModel>()!;
        string? clientId = httpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("client_id", StringComparison.CurrentCultureIgnoreCase))?.Value;
        List<ApplicationIdentifier> applicationIdentifiers = await cacheService.GetApplicationIdentifier();

        ApplicationIdentifier currentClient = applicationIdentifiers.Where(x => x.IdpClientId == clientId).FirstOrDefault();

        string? path = httpContext.Request.Path.Value?.ToLower();
        switch (path)
        {
            case string x when path!.Contains("api/externalservices"):
                if (!httpContext.User.Claims.Any(c => c.Type == "scope" && c.Value.Equals(Scope.CPG_ExternalService, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Unauthorized(httpContext);
                    return;
                }
                break;

            case string x when ContainsAny(path, anonymousApis):
                await next(httpContext);
                return;

            default:
                if (!httpContext.User.Claims.Any(c => c.Type == "scope" && c.Value.Equals(Scope.CPG, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Unauthorized(httpContext);
                    return;
                }
                break;
        }

        if (currentClient == null)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized Client : " + clientId);
            return;
        }

        AuthenticateResult result = await httpContext.AuthenticateAsync();
        httpContext.User = ClonePrincipal(result?.Principal!, currentClient.Id.ToString());
        if (path!.Contains("api/externalservices"))
        {
            await AddExternalServiceCallLog(httpContext);
        }
        else
        {
            await next(httpContext);
        }
    }

    private static ClaimsPrincipal ClonePrincipal(ClaimsPrincipal principal, string idpclientid)
    {
        ClaimsPrincipal clone = principal.Clone();
        ClaimsIdentity? newIdentity = clone.Identity as ClaimsIdentity;
        newIdentity?.AddClaim(new Claim(type: "api-client-Id", value: idpclientid));
        return clone;
    }

    private static void Unauthorized([NotNull] HttpContext httpContext)
    {
        httpContext.Response.Clear();
        httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        _ = httpContext.Response.WriteAsync("Unauthorized");
    }

    private async Task AddExternalServiceCallLog(HttpContext httpContext)
    {
        _ = long.TryParse(httpContext!.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);
        string? clientId = httpContext!.User.Claims.FirstOrDefault(c => c.Type.ToLower() == "client_id")?.Value;
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Position = 0;
        using StreamReader reqStream = new(httpContext.Request.Body);
        string requestBody = await reqStream.ReadToEndAsync();

        var originalBodyStream = httpContext.Response.Body;
        using MemoryStream memoryStream = new();
        httpContext.Response.Body = memoryStream;
        await next(httpContext);
        memoryStream.Position = 0;
        using StreamReader resStream = new(memoryStream);
        string responseBody = await resStream.ReadToEndAsync();
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalBodyStream);
        httpContext.Response.Body = originalBodyStream;

        ExternalServiceCallLog externalServiceCallLog = new()
        {
            RequestBody = requestBody,
            ResponseBody = responseBody,
            ServiceCallDate = DateTime.Now,
            ClientId = clientId,
            ServiceCallUrl = httpContext.Request.Path.Value?.ToLower(),
            ServiceCallStatusCode = httpContext.Response.StatusCode,
        };
        logger.LogInformation("[ExternalServiceCallLog] {@ExternalServiceCallLog}", externalServiceCallLog);
    }

    private static bool ContainsAny(string mainString, List<string> substrings)
    {
        foreach (string substring in substrings) { if (mainString.Contains(substring, StringComparison.CurrentCultureIgnoreCase)) { return true; } }
        return false;
    }
}
