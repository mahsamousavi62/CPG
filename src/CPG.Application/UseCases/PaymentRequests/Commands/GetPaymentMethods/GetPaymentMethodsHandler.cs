using CPG.Application.UseCases.PaymentRequests.ViewModels;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public abstract class GetPaymentMethodsHandler
{
    protected GetPaymentMethodsHandler handler;
    public void SetNextHandler(GetPaymentMethodsHandler h) => this.handler = h;
    public abstract Task HandleRequset(PaymentMethodType methodType, RequestContext request, PaymentMethodsViewModel model);
}
