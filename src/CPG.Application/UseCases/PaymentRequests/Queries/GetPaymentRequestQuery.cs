using CPG.Application.UseCases.PaymentRequests.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Queries
{
    public class GetPaymentRequestQuery:IRequest<IReadOnlyCollection<PaymentRequestViewModel>>
    {
    }
}
