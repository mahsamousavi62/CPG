using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.Exceptions;

namespace CPG.PaymentRequestAggregate.UseCases.PaymentRequests.Exceptions;

public class SourceIbanEmptyException : DomainException
{
	public override string Code => "";

	public SourceIbanEmptyException() : base(Resource.PaymentRequestInActiveCompany)
	{

	}
}