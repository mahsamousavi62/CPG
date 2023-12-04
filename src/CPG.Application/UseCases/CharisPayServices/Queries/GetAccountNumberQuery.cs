using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CharisPayServices.Queries
{
    public class GetAccountNumberQuery(string iban):IRequest<ResultData<AccountNumberViewModel>>
    {
        public string Iban { get; set; } = iban;
    }
}
