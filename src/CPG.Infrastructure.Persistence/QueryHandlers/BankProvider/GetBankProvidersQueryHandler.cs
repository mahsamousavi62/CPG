using Ardalis.GuardClauses;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.BankProvider;

public class GetBankProvidersQueryHandler(ReadDbContext context) : IRequestHandler<GetBankProvidersQuery, IReadOnlyCollection<BankProviderViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<BankProviderViewModel>> Handle(GetBankProvidersQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.ProviderType, nameof(request.ProviderType));

        var bankProviders = await _context.BankProviderReadModels.Where(x => x.ProviderType == request.ProviderType && x.IsActive).Include(x => x.Bank).Select(x => new BankProviderViewModel
        {
            Id = x.Id,
            BankId = x.BankId,
            ProviderType = x.ProviderType,
            BankName = x.Bank.Name,
        }).ToListAsync(cancellationToken: cancellationToken);

        return bankProviders;
    }
}