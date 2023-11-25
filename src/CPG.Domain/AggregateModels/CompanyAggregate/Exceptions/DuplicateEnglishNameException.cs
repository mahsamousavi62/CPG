using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    internal class DuplicateEnglishNameException(string name) : DomainException(string.Format(Resource.Duplicate_EnglishName, name))
    {
        public override string Code => "Duplicate_EnglishName";
        public string Name { get; } = name;
    }
}
