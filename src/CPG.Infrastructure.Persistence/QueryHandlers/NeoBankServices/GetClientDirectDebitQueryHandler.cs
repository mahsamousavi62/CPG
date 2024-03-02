using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.NeoBankServices
{
    public class GetClientDirectDebitQueryHandler(INeoBankService neoBankService) : IRequestHandler<GetClientDirectDebitQuery, Result<ClientDirectDebitResponse>>
    {
        private readonly INeoBankService neoBankService = neoBankService;

        public async Task<Result<ClientDirectDebitResponse>> Handle(GetClientDirectDebitQuery request, CancellationToken cancellationToken)
        => await neoBankService.ClientDirectDebit();
    }

}
