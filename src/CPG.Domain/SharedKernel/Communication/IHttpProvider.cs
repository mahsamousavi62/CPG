using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using Microsoft.AspNetCore.Http;

namespace CPG.Domain.SharedKernel.Communication
{
    public interface IHttpProvider
    {
        Task<TResponse?> PostAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
           where TResponse : ResponseBase
           where TError : ResponseBase
           where TBaseRequest : RequestBase
           where TBody : class;

        Task<TResponse?> PostAsync3<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
           where TResponse : ResponseBase
           where TError : ResponseBase
           where TBaseRequest : RequestBase
           where TBody : class;

        Task<TResponse?> PostAsync4<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
          where TResponse : ResponseBase
          where TError : ResponseBase
          where TBaseRequest : RequestBase
          where TBody : class;

        Task<TResponse?> PutAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase?
            where TError : ResponseBase
            where TBaseRequest : RequestBase
            where TBody : class;

        Task<TResponse?> PatchAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase?
            where TError : ResponseBase
            where TBaseRequest : RequestBase
            where TBody : class;

        Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError>(HttpProviderRequest<dynamic>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase
            where TError : ResponseBase
            where TBaseRequest : RequestBase;

        Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase
            where TError : ResponseBase
            where TBaseRequest : RequestBase
            where TBody : class;

        Task<TResponse?> GetAsync<TRequest, TResponse, TBody>([NotNull] HttpProviderRequest<TBody, TRequest> request, Func<HttpResponseMessage, Task>? postCallHandler = null,
                 Func<HttpResponseMessage, Task<TResponse?>>? decodeHandler = null, Func<TRequest?, TResponse?, Task<TResponse?>>? failHandler = null)
                 where TRequest : class, IHttpRequest
                 where TResponse : class, IHttpResponse
                 where TBody : class;
    }
}
