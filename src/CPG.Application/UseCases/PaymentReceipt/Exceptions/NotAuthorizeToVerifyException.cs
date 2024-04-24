using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentReceipt.Exceptions;

public class NotAuthorizeToVerifyException() : AppException(string.Format(GlobalResource.NotAuthorizeToVerify))
{
    public override string Code => "1015003";
}
