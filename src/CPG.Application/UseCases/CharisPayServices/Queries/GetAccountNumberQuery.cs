using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CharisPayServices.Queries
{
    public class GetAccountNumberQuery(string iban):IRequest<ResultData<AccountNumberResponse>>
    {
        public string Iban { get; set; } = iban;
    }
}
