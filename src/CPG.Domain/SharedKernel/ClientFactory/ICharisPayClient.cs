using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.SharedKernel;
using System.Threading.Tasks;

namespace Api.Juros.Infrastructure.External
{
    public interface ICharisPayClient
    {
        Task<ResultData<AccountNumberViewModel>> GetAccountNumber(string iban);
    }
}
