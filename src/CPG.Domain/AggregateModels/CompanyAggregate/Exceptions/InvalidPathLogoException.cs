using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidPathLogoException:DomainException
    {
        public override string Code => "invalid_path_logo";
        public InvalidPathLogoException(string message) : base(message)
        {
        }

    }
}
