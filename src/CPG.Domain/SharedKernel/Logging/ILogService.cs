using System.Net.Http;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Logging;

public interface ILogService
{
    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderType ProviderType { get; set; }

    void AddServiceCallLog(string request, string response, short status, string message);
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response,
        string resString);
}
