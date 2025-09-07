using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.Exceptions;

namespace CPG.PaymentRequestAggregate.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInActiveCompanyException : DomainException
{
	public override string Code => "1003001";

	public PaymentRequestInActiveCompanyException() : base(Resource.PaymentRequestInActiveCompany)
	{

	}
}