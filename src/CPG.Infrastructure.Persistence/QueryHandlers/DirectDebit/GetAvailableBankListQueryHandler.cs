using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.DirectDebit.Query;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.DirectDebit;

public class GetAvailableBankListQueryHandler(ReadDbContext contebankt, IMinioProvider minioProvider) : IRequestHandler<GetAvailableBankListQuery, Result<IReadOnlyCollection<AvailableBankViewModel>>>
{
    private readonly ReadDbContext _contebankt = contebankt;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<AvailableBankViewModel>>> Handle(GetAvailableBankListQuery request, CancellationToken cancellationToken)
    {
        var banks = await _contebankt.BankReadModels.Where(t => t.IsActive && t.HasDirectDebitFeature == true && t.DirectDebitSetting.Provider.IsActive &&
            t.DirectDebitSetting.Provider.PaymentMethods.Any(q => q.MethodType == Enums.PaymentMethodType.DirectDebit) && t.DirectDebitSetting.IsActive)
            .Include(t => t.DirectDebitSetting)
            .ThenInclude(t => t.Provider)
            .ThenInclude(t => t.PaymentMethods)
            .ToListAsync(cancellationToken);

        var bankViewModels = await Task.WhenAll(
            banks.Select(async bank => new AvailableBankViewModel
            {
                Id = bank.Id,
                Name = bank.Name,
                Logo = !string.IsNullOrEmpty(bank.Logo) ? await _minioProvider.PresignedGetObject(bank.Logo) : "",
            }))
            .ConfigureAwait(false);

        return Result<IReadOnlyCollection<AvailableBankViewModel>>.SuccessResult(bankViewModels);
    }
}