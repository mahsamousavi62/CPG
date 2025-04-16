using Ardalis.GuardClauses;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;

public class GetCompanyDepositsByPaymentCodeQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyDepositsByPaymentCodeQuery, Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> Handle(GetCompanyDepositsByPaymentCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Guard.Against.NullOrWhiteSpace(request.PaymentCode, nameof(request.PaymentCode));

            var paymentRequest = await _context.PaymentRequestReadModels
                .Include(c => c.PaymentRequestMethods.Where(p => p.IsActive))
                .ThenInclude(d => d.PaymentRequestMethodDeposits.Where(p => p.IsActive))
                .FirstOrDefaultAsync(t => t.PaymentCode == request.PaymentCode);

            if (paymentRequest == null)
                throw new PaymentRequestCodeNotFoundException();

            var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.PaymentReceipt);

            var companyDeposits = await _context.CompanyDepositReadModels
                .Include(t => t.PaymentMethods)
                .Include(t => t.Company)
                .Include(t => t.Bank)
                .Where(t => t.CompanyId == paymentRequest.CompanyId
                    && t.IsActive
                    && t.Bank.IsActive
                    && t.PaymentMethods.Any(p => p.MethodType == PaymentMethodType.PaymentReceipt))
                .ToListAsync();

            var selectedDeposits = paymentRequestMethod?.PaymentRequestMethodDeposits.Any() == true
                ? companyDeposits.Where(deposit => paymentRequestMethod.PaymentRequestMethodDeposits
                    .Select(x => x.CompanyDepositId)
                    .Contains(deposit.Id))
                : companyDeposits;

            var companyViewModels = await Task.WhenAll(
                selectedDeposits.Select(async deposit => new CompanyDepositViewModel
                {
                    Id = deposit.Id,
                    Name = deposit.Name,
                    AccountNumber = deposit.AccountNumber,
                    Iban = deposit.Iban,
                    BankId = deposit.BankId,
                    BankLogo = await General.GetLogo(_minioProvider, deposit.Bank.Logo),
                    BankName = deposit.Bank.Name,
                    CompanyId = deposit.CompanyId,
                    CompanyName = deposit.Company.PersianName,
                    IsDefaultForDirectDebit = deposit.IsDefaultForDirectDebit,
                    CreationDate = deposit.CreationDate,
                    IsActive = deposit.IsActive,
                    ModificationDate = deposit.ModificationDate,
                    PaymentMethods = deposit.PaymentMethods.Select(p => p.MethodType).ToList(),
                }))
                .ConfigureAwait(false);

            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.SuccessResult(companyViewModels);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.Failure(new Error(ex.Source, ex.Message));
        }
    }
}
