using CPG.Application.UseCases.CharisPayServices.Queries;
using System.Threading.Tasks;

namespace Api.Juros.Infrastructure.External
{
    public interface ICharisPayClient
    {
        Task<AccountNumberViewModel> GetAccountNumber(string iban);
    }
}
