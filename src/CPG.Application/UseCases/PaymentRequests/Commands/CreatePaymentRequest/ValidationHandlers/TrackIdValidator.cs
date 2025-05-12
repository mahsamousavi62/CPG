using CPG.Application.UseCases.PaymentRequests.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers
{
    internal class TrackIdValidator<T> : ValidatorHandler<T>
        where T : PaymentRequest
    {        
        public override void Handle(T sameTrackerId)
        {
            if (sameTrackerId != null)
                throw new PaymentRequestDuplicateTrackerIdException(sameTrackerId.TrackerId);
        }
    }
}
