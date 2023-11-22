using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyEnglishNameException:DomainException
    {
        public override string Code => "empty_englishName";
        public EmptyEnglishNameException(string message) : base(message)
        {
        }

    }
}
