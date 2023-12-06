using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidEnglishNameCharachterException(string name) :
    DomainException(string.Format(Resource.Invalid_EnglishNameCharachterLimit, name))
    {
        public override string Code => "Invalid_EnglishNameCharachterLimit";
    }
}
