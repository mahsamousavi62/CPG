using System;
using System.Net.Http;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class AsanPardakhtProvider(IHttpClientFactory factory, IConfiguration configuration) : IIpgProvider
    {
        private readonly IHttpClientFactory _factory = factory;
        private readonly IConfiguration _configuration = configuration;

        public Task<PaymentTicketResponse> GetPaymentTicketAsync(PaymentTicketRequest paymentIpgRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var client = _factory.CreateClient("charisPayClient");
            client.SetBearerToken(configuration["Infrastructure:CharisPay:Token"]);
            return await Task.FromResult(new TransactionResultResponse());
        }
    }
}
