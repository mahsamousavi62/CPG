using CPG.Domain.SharedKernel.Communication;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Json;
using Newtonsoft.Json;
using MassTransit;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using System.Diagnostics.CodeAnalysis;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Infrastructure.Providers;

public class HttpProvider : IHttpProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogService _logService;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpProvider(IHttpClientFactory httpClientFactory, ILogService logService, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse?> PostAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string requestBodyJson = null;
        if (request?.Body != null)
        {
            requestBodyJson = System.Text.Json.JsonSerializer.Serialize(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (var i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.QueryParameters != null)
            {
                var queryParams = GetQueryParameters(request.QueryParameters);
                request.Uri += "?" + queryParams;
            }

            var response = await client.PostAsJsonAsync(request.Uri, request.Body);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            TResponse result = null;
            if (decoder != null)
            {
                result = decoder(resString);
            }
            else if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }

            TError? errorResult = result as TError;
            if (result is null)
            {
                errorResult = await response.Content.ReadFromJsonAsync<TError>();
                if (errorResult is null)
                {
                    throw new Exception($"value is not instance of {nameof(TResponse)}");
                }
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK && errorHandler is not null)
            {
                return await errorHandler(baseRequest, result, errorResult, (short)response.StatusCode);
            }
            result.StatusCode = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(PostAsync), exc, request?.Uri, requestBodyJson);
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TResponse?> PostAsync3<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string json = null;
        if (request?.Body != null)
        {
            json = JsonConvert.SerializeObject(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (var i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.QueryParameters != null)
            {
                var queryParams = GetQueryParameters(request.QueryParameters);
                request.Uri += "?" + queryParams;
            }

            var content = new StringContent(json, null, "application/json-patch+json");
            var response = await client.PostAsync(request.Uri, content);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            TResponse result = null;
            if (decoder != null)
            {
                result = decoder(resString);
            }
            else if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }

            TError? errorResult = result as TError;
            if (result is null)
            {
                errorResult = await response.Content.ReadFromJsonAsync<TError>();
                if (errorResult is null)
                {
                    throw new Exception($"value is not instance of {nameof(TResponse)}");
                }
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK && errorHandler is not null)
            {
                return await errorHandler(baseRequest, result, errorResult, (short)response.StatusCode);
            }
            result.StatusCode = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(PostAsync3), exc, request?.Uri, json);
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TResponse?> PostAsync4<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string json = null;
        if (request?.Body != null)
        {
            json = System.Text.Json.JsonSerializer.Serialize(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (var i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.QueryParameters != null)
            {
                var queryParams = GetQueryParameters(request.QueryParameters);
                request.Uri += "?" + queryParams;
            }

            var content = new StringContent(json, null, "application/json");
            var response = await client.PostAsync(request.Uri, content);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            TResponse result = null;
            if (decoder != null)
            {
                result = decoder(resString);
            }
            else if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }

            TError? errorResult = result as TError;
            if (result is null)
            {
                errorResult = await response.Content.ReadFromJsonAsync<TError>();
                if (errorResult is null)
                {
                    throw new Exception($"value is not instance of {nameof(TResponse)}");
                }
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK && errorHandler is not null)
            {
                return await errorHandler(baseRequest, result, errorResult, (short)response.StatusCode);
            }
            result.StatusCode = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(PostAsync4), exc, request?.Uri, json);
            _logService.LogError(callLog);
            throw;
        }
    }

    //public async Task<TResponse?> PostAsync2<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, Task<TResponse?>>? errorHandler)
    //   where TResponse : ResponseBase
    //   where TError : ResponseBase
    //   where TBaseRequest : RequestBase
    //{
    //    try
    //    {
    //        if (request is null)
    //        {
    //            throw new ArgumentNullException(nameof(request));
    //        }

    //        var client = _httpClientFactory.CreateClient();

    //        client.BaseAddress = new Uri(request.BaseAddress ?? "");
    //        if (request.HeaderParameters?.Any() == true)
    //        {
    //            for (var i = 0; i < request.HeaderParameters.Count; i++)
    //            {
    //                client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
    //            }
    //        }

    //        if (request.QueryParameters != null)
    //        {
    //            var queryParams = GetQueryParameters(request.QueryParameters);
    //            request.Uri += "?" + queryParams;
    //        }

    //        var response = await client.PostAsJsonAsync(request.Uri, request.Body);

    //        var resString = await response.Content.ReadAsStringAsync();
    //        //infraHelper.Value.AddServiceCallLog(request, response, resString);

    //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            TError? result = null;
    //            try
    //            {
    //                result = await response.Content.ReadFromJsonAsync<TError>();
    //            }
    //            catch
    //            {
    //                if (resString.StartsWith("{\"message\":"))
    //                {
    //                    var res = resString.Split("\"error\":")[1];
    //                    result = JsonSerializer.Deserialize<TError>(res.Remove(res.Length - 1));
    //                }
    //            }
    //            return errorHandler == null ? null : await errorHandler(baseRequest, null, result);
    //        }
    //    }
    //    catch (Exception exc)
    //    {
    //        _logger.LogError(exc, nameof(PostAsync));

    //        throw;
    //    }
    //}

    public async Task<TResponse?> PutAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase?
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string requestBodyJson = null;
        if (request?.Body != null)
        {
            requestBodyJson = System.Text.Json.JsonSerializer.Serialize(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (var i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.QueryParameters != null)
            {
                var queryParams = GetQueryParameters(request.QueryParameters);
                request.Uri += "?" + queryParams;
            }

            var response = await client.PutAsJsonAsync(request.Uri, request.Body);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<TError>();
                result.StatusCode = (short)response.StatusCode;

                return errorHandler == null ? null :
                    await errorHandler(baseRequest, null, result, (short)response.StatusCode);
            }
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(PutAsync), exc, request?.Uri, requestBodyJson);
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TResponse?> PatchAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string requestBodyJson = null;
        if (request?.Body != null)
        {
            requestBodyJson = System.Text.Json.JsonSerializer.Serialize(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (var i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.QueryParameters != null)
            {
                var queryParams = GetQueryParameters(request.QueryParameters);
                request.Uri += "?" + queryParams;
            }

            var response = await client.PatchAsJsonAsync(request.Uri, request.Body);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            TResponse result = null;
            if (decoder != null)
            {
                result = decoder(resString);
            }
            else if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadFromJsonAsync<TResponse>();
            }

            TError? errorResult = result as TError;
            if (result is null)
            {
                errorResult = await response.Content.ReadFromJsonAsync<TError>();
                if (errorResult is null)
                {
                    throw new Exception($"value is not instance of {nameof(TResponse)}");
                }
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK && errorHandler is not null)
            {
                return await errorHandler(baseRequest, result, errorResult, (short)response.StatusCode);
            }
            result.StatusCode = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(PatchAsync), exc, request?.Uri, requestBodyJson);
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError>(HttpProviderRequest<dynamic>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        return await GetAsync<TBaseRequest, TResponse, TError, dynamic>(request, baseRequest, errorHandler, decoder);
    }

    public async Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        // Serialize request body before try block for error logging
        string requestBodyJson = null;
        if (request?.Body != null)
        {
            requestBodyJson = System.Text.Json.JsonSerializer.Serialize(request.Body);
        }

        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(request.BaseAddress ?? "");
            if (request.HeaderParameters?.Any() == true)
            {
                for (int i = 0; i < request.HeaderParameters.Count; i++)
                {
                    client.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
                }
            }

            if (request.Body != null)
            {
                var queryParams = GetQueryParameters(request.Body);
                request.Uri += "?" + queryParams;
            }

            var response = await client.GetAsync(request.Uri);

            var resString = await response.Content.ReadAsStringAsync();
            _logService.AddServiceCallLog(request, response, resString);

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            var errorResult = result as TError;
            if (result is null)
            {
                errorResult = await response.Content.ReadFromJsonAsync<TError>();
                if (errorResult is null)
                {
                    throw new Exception($"value is not instance of {nameof(TResponse)}");
                }
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (errorHandler is not null)
                {
                    return await errorHandler(baseRequest, result, errorResult, (short)response.StatusCode);
                }
            }
            result.StatusCode = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(GetAsync), exc, request?.Uri, requestBodyJson);
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TResponse?> GetAsync<TRequest, TResponse, TBody>([NotNull] HttpProviderRequest<TBody, TRequest> request, Func<HttpResponseMessage, Task>? postCallHandler = null,
               Func<HttpResponseMessage, Task<TResponse?>>? decodeHandler = null, Func<TRequest?, TResponse?, Task<TResponse?>>? failHandler = null)
               where TRequest : IHttpRequest
               where TResponse : IHttpResponse
    {
        // Serialize request object before try block for error logging
        string requestJson = null;
        if (request?.Request != null)
        {
            requestJson = System.Text.Json.JsonSerializer.Serialize(request.Request);
        }

        var client = _httpClientFactory.CreateClient();
        if (!string.IsNullOrEmpty(request.BaseAddress))
        {
            client!.BaseAddress = new Uri(request.BaseAddress);
        }

        if (request.HeaderParameters?.Count > 0)
        {
            for (int i = 0; i < request.HeaderParameters.Count; i++)
            {
                client!.DefaultRequestHeaders.Add(request.HeaderParameters[i].Key, request.HeaderParameters[i].Value);
            }
        }

        HttpResponseMessage response = await client!.GetAsync(request.Uri);

        var resString = await response.Content.ReadAsStringAsync();
        if (postCallHandler is not null)
        {
            await postCallHandler(response);
        }

        TResponse? result;
        try
        {
            result = decodeHandler is not null ? await decodeHandler(response) : await response.Content.ReadFromJsonAsync<TResponse>();
            result.StatusCode = (short)response.StatusCode;
            return response.StatusCode is not System.Net.HttpStatusCode.OK && failHandler is not null ? await failHandler(request.Request, result) : result;
        }
        catch (Exception exc)
        {
            var callLog = CreateErrorCallLogModel(nameof(GetAsync), exc, request?.Uri, requestJson);
            _logService.LogError(callLog);
            throw;
        }
        finally
        {
            await _logService.AddServiceCallLogAsync(request, response);
            client?.Dispose();
        }
    }


    private static string GetPropertyName(PropertyInfo property)
    {
        string name = property.Name;
        var jsonAttribute = property.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
        if (jsonAttribute != null)
        {
            name = jsonAttribute.Name;
        }
        return name;
    }

    private static string GetQueryParameters(object request)
    {
        var properties = request.GetType().GetProperties();
        var lst = properties
            .Where(t => t.GetValue(request) != null)
            .Select(t => HttpUtility.UrlEncode(GetPropertyName(t)) + "=" + HttpUtility.UrlEncode(Convert.ToString(t.GetValue(request), CultureInfo.InvariantCulture))).ToArray();
        var queryParams = string.Join("&", lst);
        return queryParams;
    }

    private void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString)
    {
        if (!string.IsNullOrEmpty(resString))
        {
            resString = Regex.Replace(resString, Constants.Pattern, Constants.Replaceformat);
        }
        string reqString = System.Text.Json.JsonSerializer.Serialize(request);
        if (!string.IsNullOrEmpty(reqString))
        {
            reqString = Regex.Replace(reqString, Constants.Pattern, Constants.Replaceformat);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callLog = response.IsSuccessStatusCode
            ? CallLogModel.CreateSuccess(
                serviceName: request.Service?.ToString() ?? "HttpProvider",
                providerName: request.Provider?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                serviceType: request.Service,
                providerType: request.Provider,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: (int)response.StatusCode
            )
            : CallLogModel.CreateError(
                serviceName: request.Service?.ToString() ?? "HttpProvider",
                providerName: request.Provider?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                exception: null,
                serviceType: request.Service,
                providerType: request.Provider,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );

        _logService.LogInformation(callLog);
    }

    private CallLogModel CreateErrorCallLogModel(string methodName, Exception exception, string url, string requestBody = null, string responseBody = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        // Apply PII masking to requestBody if provided
        string maskedRequestBody = requestBody;
        if (!string.IsNullOrEmpty(maskedRequestBody))
        {
            maskedRequestBody = Regex.Replace(maskedRequestBody, Constants.Pattern, Constants.Replaceformat);
        }

        // Apply PII masking to responseBody if provided
        string maskedResponseBody = responseBody;
        if (!string.IsNullOrEmpty(maskedResponseBody))
        {
            maskedResponseBody = Regex.Replace(maskedResponseBody, Constants.Pattern, Constants.Replaceformat);
        }

        return CallLogModel.CreateError(
            serviceName: "HttpProvider",
            providerName: methodName,
            requestUri: url ?? methodName,
            requestBody: maskedRequestBody,
            responseBody: maskedResponseBody ?? exception.Message,
            exception: exception,
            auditType: Enums.AuditType.Provider,
            correlationId: httpContext?.TraceIdentifier,
            userId: userId,
            applicationId: applicationId,
            companyId: companyId,
            ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
            userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
        );
    }
}
