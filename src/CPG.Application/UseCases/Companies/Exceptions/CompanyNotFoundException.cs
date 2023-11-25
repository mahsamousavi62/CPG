using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Exceptions;
public class CompanyNotFoundException : ApplicationException
{
    public long CompanyId { get; }

    public CompanyNotFoundException(long companyId) : base($"Company with ID {companyId} has not been found.")
        => CompanyId=companyId;
}
