using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyPersianNameException:DomainException
    {
        public override string Code => "empt♂y_persianName";
        public EmptyPersianNameException(string message) : base(message)
        {
        }

    }
}
