using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Bank;

public class GetAllBanksQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllBanksQuery, IReadOnlyCollection<BankViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyCollection<BankViewModel>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var banks = await _context.BankReadModels.ToListAsync(cancellationToken: cancellationToken);

        return await Task.WhenAll(banks.Select(async x => new BankViewModel
        {
            Id = x.Id,
            Name = x.Name,
            IbanPrefix = x.IbanPrefix,
            Logo = !string.IsNullOrEmpty(x.LogoAddress) ? await _minioProvider.PresignedGetObject(x.LogoAddress) : "",
            IsActive = x.IsActive,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate
        })).ConfigureAwait(false);
    }
}
