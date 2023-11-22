using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyLogoException:DomainException
    {
        public override string Code => "empty_logo";
        public EmptyLogoException(string message) : base(message)
        {
        }

    }
}
