using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions
{
    internal class BankIsActiveException : DomainException
    {
        public override string Code => "bank_is_already_active";
        public long BookId { get; }

        public BankIsActiveException(long bookId) : base(string.Format(Resource.BankIsAlreadyActive, bookId))
           => BookId = bookId;
    }
}
