using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using System.Threading.Tasks;

namespace CPG.Application.Shared.Interfaces;

public interface INotificationHub
{
    public Task SendMessage(Result<WithdrawalDataResponseViewModel> result);
}
