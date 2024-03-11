using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetCompanyDepositsByPaymentCodeQuery(string paymentCode) : IRequest<Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    public string PaymentCode = paymentCode;
}