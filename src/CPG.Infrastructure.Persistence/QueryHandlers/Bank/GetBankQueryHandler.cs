using Ardalis.GuardClauses;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Bank;

public class GetBankQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetBankQuery, Result<BankViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<BankViewModel>> Handle(GetBankQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.BankId, nameof(request.BankId));

        var bank = await _context.BankReadModels.Include(t => t.DirectDebitSetting).FirstOrDefaultAsync(t => t.Id == request.BankId);
        if (bank == null)
            throw new BankNotFoundException(request.BankId);

        var bankModel = new BankViewModel
        {
            Id = bank.Id,
            Name = bank.Name,
            IbanPrefix = bank.IbanPrefix,
            Logo = !string.IsNullOrEmpty(bank.LogoAddress) ? await _minioProvider.PresignedGetObject(bank.LogoAddress) : "",
            IsActive = bank.IsActive,
            HasDirectDebitFeature = bank.HasDirectDebitFeature,
            DirectDebitSetting = bank.DirectDebitSetting != null ? new BankDirectDebitSettingViewModel
            {
                Id = bank.DirectDebitSetting.Id,
                AuthenticationType = bank.DirectDebitSetting.AuthenticationType,
                BankId = bank.DirectDebitSetting.BankId,
                DDBankCode = bank.DirectDebitSetting.DDBankCode,
                MaxMandateValidityDurationPerMonth = bank.DirectDebitSetting.MaxMandateValidityDurationPerMonth,
                MaxWithdrawalAmountPerDay = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay,
                ProviderId = bank.DirectDebitSetting.ProviderId
            } : null,
        };

        return Result<BankViewModel>.SuccessResult(bankModel);
    }
}