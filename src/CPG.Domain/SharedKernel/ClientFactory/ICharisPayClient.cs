using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.SharedKernel;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ClientFactory;

    public interface ICharisPayClient
    {
        Task<ResultData<AccountNumberViewModel>> GetAccountNumber(string iban);
    }

