using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Bank;

public class GetAllBanksQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllBanksQuery, Result<IReadOnlyCollection<BankViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<BankViewModel>>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var banks = await _context.BankReadModels.Include(t => t.DirectDebitSetting).ThenInclude(t => t.Provider).ToListAsync(cancellationToken: cancellationToken);

        var data = await Task.WhenAll(banks.Select(async x => new BankViewModel
        {
            Id = x.Id,
            Name = x.Name,
            IbanPrefix = x.IbanPrefix,
            Logo = !string.IsNullOrEmpty(x.Logo) ? await _minioProvider.PresignedGetObject(x.Logo) : "",
            HasDirectDebitFeature = x.HasDirectDebitFeature,
            IsActive = x.IsActive,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<BankViewModel>>.SuccessResult(data);
    }
}
