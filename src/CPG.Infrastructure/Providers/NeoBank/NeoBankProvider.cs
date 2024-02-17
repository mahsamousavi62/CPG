using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel;
using System.Net;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using CPG.Domain.SharedKernel.Communication.Charispay.Models;
using CPG.Infrastructure.Providers.Charispay;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System;

namespace CPG.Infrastructure.Providers.NeoBank;

public class NeoBankProvider(IHttpClientFactory factory, IConfiguration configuration) : INeoBankService
{
    private readonly IHttpClientFactory factory = factory;
    private readonly IConfiguration configuration = configuration;

    public async Task<ResultData<UserDepositBalanceResponse>> GetUserDepositBalance()
    {
        var neobankConfig = configuration.Get<NeoBankConfig>();
        ResultData<UserDepositBalanceResponse> resultData = new();

        var client = factory.CreateClient("neoBankClient");

        string exchageToken = TokenExchane();
        client.SetBearerToken(exchageToken);

        var result = await client.GetAsync(neobankConfig.UserDepositBalanceUrl);

        var resultContent = await result.Content.ReadAsStringAsync();
        try
        {
            var response = JsonConvert.DeserializeObject<UserDepositBalanceResponse>(resultContent);
            resultData.Data = response;
            resultData.OperationResult = Enums.OperationResult.Succeeded;
        }
        catch (Exception)
        {
            dynamic d = JObject.Parse(resultContent);

            resultData.Error = d.result;
            resultData.OperationResult = Enums.OperationResult.Failed;
        }
        return resultData;

    }

    private string TokenExchane()
    {
        throw new NotImplementedException();
    }
}
