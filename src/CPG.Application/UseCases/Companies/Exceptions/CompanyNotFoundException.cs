using System;

namespace CPG.Application.UseCases.Companies.Exceptions;

public class CompanyNotFoundException(long companyId) : ApplicationException($"Company with ID {companyId} has not been found.")
{
    public long CompanyId { get; } = companyId;
}
