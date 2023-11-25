using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class MaximalFileSizeException : DomainException
{
    public override string Code => "MaximalFileSize";
    public MaximalFileSizeException(string message) : base(message)
    {
    }
}
