using System;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositNotFoundException(long companyDepositId) : ApplicationException($"CompanyDeposit with ID {companyDepositId} has not been found.")
{
    public long CompanyDepositId { get; } = companyDepositId;
}
