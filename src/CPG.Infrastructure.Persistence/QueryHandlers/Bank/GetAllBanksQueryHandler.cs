using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Bank;

public class GetAllBanksQueryHandler(ReadDbContext context) : IRequestHandler<GetAllBanksQuery, IReadOnlyCollection<BankViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<BankViewModel>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var banks = await _context.BankReadModels.Select(x => new BankViewModel
        {
            Id = x.Id,
            Name = x.Name,
            IbanPrefix = x.IbanPrefix,
            LogoAddress = x.LogoAddress,            
            IsActive = x.IsActive,
        }).ToListAsync(cancellationToken: cancellationToken);

        return banks;
    }
}
