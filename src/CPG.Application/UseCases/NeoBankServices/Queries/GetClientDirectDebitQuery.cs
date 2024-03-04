using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.NeoBankServices.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;

namespace CPG.Application.UseCases.NeoBankServices.Queries
{
    public class GetClientDirectDebitQuery(ClientDirectDebitRequest model) :IRequest<Result<ClientDirectDebitResponse>>
    {
        public ClientDirectDebitRequest Model { get; set; } = model;
    }
}
