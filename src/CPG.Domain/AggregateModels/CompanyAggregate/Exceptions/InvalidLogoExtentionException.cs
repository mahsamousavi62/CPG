using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    internal class InvalidLogoExtentionException : DomainException
    {
        public override string Code => "invalid_logo_extention";
        public InvalidLogoExtentionException(string message) : base(message)
        {
        }
    }
}
