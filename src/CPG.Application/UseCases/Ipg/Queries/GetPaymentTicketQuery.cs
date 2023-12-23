using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries
{
    public class GetPaymentTicketQuery(PaymentTicketRequest paymentTicketRequest) : IRequest<ResultData<PaymentTicketResponse>>
    {
        public PaymentTicketRequest PaymentTicketRequest { get; set; }=paymentTicketRequest;
    }
}
