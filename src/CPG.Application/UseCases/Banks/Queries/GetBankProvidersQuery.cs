using CPG.Application.UseCases.Banks.ViewModels;
using MediatR;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.Queries;

public class GetBankProvidersQuery(ProviderType providerType) : IRequest<IReadOnlyCollection<BankProviderViewModel>>
{
    public ProviderType ProviderType { get; } = providerType;
}
