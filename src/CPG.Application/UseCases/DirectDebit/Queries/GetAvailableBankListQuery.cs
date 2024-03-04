using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.DirectDebit.Query;

public class GetAvailableBankListQuery() : IRequest<Result<IReadOnlyCollection<AvailableBankViewModel>>>
{
}