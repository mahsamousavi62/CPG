using System;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication
{
    public interface IHttpProvider
    {
        Task<TResponse?> PostAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
           where TResponse : ResponseBase
           where TError : ResponseBase
           where TBaseRequest : RequestBase;

        Task<TResponse?> PostAsync3<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
           where TResponse : ResponseBase
           where TError : ResponseBase
           where TBaseRequest : RequestBase;

        Task<TResponse?> PutAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase?
            where TError : ResponseBase
            where TBaseRequest : RequestBase;

        Task<TResponse?> PatchAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase?
            where TError : ResponseBase
            where TBaseRequest : RequestBase;

        Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError>(HttpProviderRequest<dynamic>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase
            where TError : ResponseBase
            where TBaseRequest : RequestBase;

        Task<TResponse?> GetAsync<TBaseRequest, TResponse, TError, TBody>(HttpProviderRequest<TBody>? request, TBaseRequest? baseRequest, Func<TBaseRequest?, TResponse?, TError?, short, Task<TResponse?>>? errorHandler, Func<string, TResponse>? decoder = null)
            where TResponse : ResponseBase
            where TError : ResponseBase
            where TBaseRequest : RequestBase;
    }
}
