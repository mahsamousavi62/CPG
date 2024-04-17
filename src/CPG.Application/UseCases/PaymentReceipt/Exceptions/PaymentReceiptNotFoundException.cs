using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentReceipt.Exceptions
{
    public class PaymentReceiptNotFoundException(long paymentReceiptId) :
    AppException(string.Format(GlobalResource.paymentReceiptNotFound, paymentReceiptId))
    {
        public override string Code => "paymentReceipt_not_found";
        public long PaymentReceiptId { get; } = paymentReceiptId;
    }
}
