using System.Net.Http;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Logging;

public interface ILogService
{
    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderType ProviderType { get; set; }
    void AddServiceCallLog(string request, string response, short status, string message);
    Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response);
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response,string resString);
}
