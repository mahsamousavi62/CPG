using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
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
        {
            if (!Regex.IsMatch(request.Model.DestinationDepositNumber, "^\\d{4}/\\d{2}/\\d{3}/\\d{9}$"))
                throw new InvalidDepositNumberFormatException(request.Model.DestinationDepositNumber);

            return await neoBankService.ClientDirectDebit(request.Model); }
    }

}
