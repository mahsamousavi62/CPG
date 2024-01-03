using CPG.Domain.SharedKernel.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json;
using MassTransit;
using Microsoft.AspNetCore.WebUtilities;
using static Azure.Core.HttpHeader;
using System.Text.RegularExpressions;
using System.Net;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Helper.CallLog;

namespace CPG.Infrastructure.Providers;

public class HttpProvider : IHttpProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpProvider> _logger;
    private readonly ILogService _logService;

    public HttpProvider(IHttpClientFactory httpClientFactory, ILogger<HttpProvider> logger,ILogService logService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _logService = logService;
    }

    public async Task<TResponse?> PostAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?,short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
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

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
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
            result.Status = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(PostAsync));

            throw;
        }
    }

    public async Task<TResponse?> PostAsync3<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?,short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
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

            string json = JsonConvert.SerializeObject(request.Body);
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

            //var result = await response.Content.ReadFromJsonAsync<TResponse>();
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
            result.Status = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(PostAsync));

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

    public async Task<TResponse?> PutAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?,short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase?
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
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

                return errorHandler == null ? null :
                    await errorHandler(baseRequest, null, result, (short)response.StatusCode);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(PutAsync));

            throw;
        }
    }

    public async Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError>(HttpProviderRequest<dynamic>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?,short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
        return await GetAsync<TBaseRequest, TResponse, TError, dynamic>(request, baseRequest, errorHandler, decoder);
    }

    public async Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?,short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
        where TResponse : ResponseBase
        where TError : ResponseBase
        where TBaseRequest : RequestBase
    {
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
            //infraHelper.Value.AddServiceCallLog(request, response, resString);

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
            result.Status = (short)response.StatusCode;
            return result;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(GetAsync));

            throw;
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
        var callLog = new
        {
            RequestBody = reqString,
            ResponseBody = resString,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = request.Uri,
            ServiceCallStatus = response.StatusCode == System.Net.HttpStatusCode.OK,
            ServiceType = request.Service,
            CreationDate = DateTime.Now,
            //ToDo: Add current user id
            CreationUserId = 1,
            ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
            ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
            Provider = request.Provider,

        };

        _logger.LogInformation("CallLog: {@CallLog}", callLog);
    }
}
