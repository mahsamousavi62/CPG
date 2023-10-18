using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions
{
    internal class BankIsSuspendedException : DomainException
    {
        public override string Code => "bank_is_already_suspended";
        public long BookId { get; }

        public BankIsSuspendedException(long bookId) : base(string.Format(Resource.BankIsAlreadySuspended, bookId))
           => BookId = bookId;
    }
}
