using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Banks.Queries;

public class GetAllBanksQuery : IRequest<Result<IReadOnlyCollection<BankViewModel>>>
{
}
