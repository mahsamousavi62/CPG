using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class CompanyCreationException : DomainException
    {
        public override string Code => "cannot_create_company";

        public CompanyCreationException(string message) : base(message)
        {
        }
    }
}
