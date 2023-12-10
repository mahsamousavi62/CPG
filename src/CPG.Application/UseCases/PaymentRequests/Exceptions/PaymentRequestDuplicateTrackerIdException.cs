using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestDuplicateTrackerIdException(string trackerId): ApplicationException
        (string.Format(GlobalResource.PaymentRequestDuplicateTrackerId))
    {
        public override string Code => "paymentRequest_duplicate_TrackerId";
}
}
