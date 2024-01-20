using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Company> companyRepository,
    IMinioProvider minioProvider) : IRequestHandler<GetPaymentMethodsCommand, Result<PaymentMethodsViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<PaymentMethodsViewModel>> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByCode(request.ViewModel.PaymentCode));

            if (paymentRequest is null)
            {
                throw new PaymentRequestCodeNotFoundException();
            }

            if (paymentRequest.UrlExpirationDateTime < DateTime.UtcNow)
            {
                throw new PaymentRequestCodeIsExpiredException();
            }

            if (paymentRequest.IsUsed)
            {
                throw new PaymentRequestCodeIsUsedException();
            }

            paymentRequest.Status = Enums.PaymentStatus.RedirectedToCpg;
            await _paymentRequestRepository.UpdateAsync(paymentRequest);

            Company company = null;
            if (!string.IsNullOrEmpty(paymentRequest.DestinationDepositIban))
            {
                company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIbanSpec(paymentRequest.CompanyId,
                    paymentRequest.DestinationDepositIban), cancellationToken);

                var toBeRemoved = new List<CompanyIPG>();
                foreach (var companyIPGItem in company?.CompanyIPGs)
                {
                    var found = companyIPGItem.IPGDeposits.Any(t => t.CompanyDeposit.Iban == paymentRequest.DestinationDepositIban);
                    if (!found)
                    {
                        toBeRemoved.Add(companyIPGItem);
                    }
                }
                foreach (var companyIPGItem in toBeRemoved)
                {
                    company.CompanyIPGs.Remove(companyIPGItem);
                }
            }
            else
            {
                company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIdSpec(paymentRequest.CompanyId), cancellationToken);

                var toBeRemoved = new List<CompanyIPG>();
                foreach (var companyIPGItem in company?.CompanyIPGs)
                {
                    var defaultDeposit = companyIPGItem.IPGDeposits.FirstOrDefault(t => t.IsDefault);
                    if (!defaultDeposit.IsActive)
                    {
                        toBeRemoved.Add(companyIPGItem);
                    }
                }
                foreach (var companyIPGItem in toBeRemoved)
                {
                    company.CompanyIPGs.Remove(companyIPGItem);
                }
            }

            if (company == null)
            {
                throw new Exception("company not found");
            }

            if (!company.IsActive)
            {
                throw new CompanyIsInactiveException(company.PersianName);
            }

            var ipgResult = await Task.WhenAll(company.CompanyIPGs?.Select(t => new { t.IPGType, t.Id }).Select(async t => new IPGInfo
            {
                Id = t.Id,
                Logo = await _minioProvider.PresignedGetObject(t.IPGType.Logo),
                PersianName = t.IPGType.PersianName,
            })).ConfigureAwait(false);

            return Result<PaymentMethodsViewModel>.SuccessResult(new PaymentMethodsViewModel
            {
                Amount = paymentRequest.Amount,
                IPGs = ipgResult?.ToList(),
                CompanyName = company.PersianName,
            });
        }
        catch (DomainException exc)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error("1006000", GlobalResource.PaymentMethodsUnexpectedError));
        }
    }
}
