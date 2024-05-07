using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestDuplicateTrackerIdException(string trackerId): AppException(string.Format(GlobalResource.PaymentRequestDuplicateTrackerId, trackerId))
{
    public override string Code => "1001033";
}
