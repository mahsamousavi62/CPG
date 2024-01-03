using System.Net.Http;
using CPG.Domain.SharedKernel.Communication;

namespace CPG.Domain.SharedKernel.Helper.CallLog;

public interface ILogService
{
    void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString);
}
