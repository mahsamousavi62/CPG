using Ardalis.GuardClauses;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Bank;

public class GetBankQueryHandler(ReadDbContext context) : IRequestHandler<GetBankQuery, BankViewModel>
{
    private readonly ReadDbContext _context = context;

    public async Task<BankViewModel> Handle(GetBankQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.BankId, nameof(request.BankId));

        var bank = await _context.BankReadModels.FirstOrDefaultAsync(t => t.Id == request.BankId);

        if (bank == null)
            throw new BankNotFoundException(request.BankId);

        var bankModel = new BankViewModel
        {
            Id = bank.Id,
            Name = bank.Name,
            IbanPrefix = bank.IbanPrefix,
            LogoAddress = bank.LogoAddress,
            IsActive = bank.IsActive,            
        };

        return bankModel;
    }
}