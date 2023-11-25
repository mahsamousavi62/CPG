using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidLogoFormatException:DomainException
    {
        public override string Code => "invalid_logo_Format";
        public InvalidLogoFormatException(string message) : base(message)
        {
        }
    }
}
