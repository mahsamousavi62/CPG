using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CancelPaymentRequet
{
    public class CancelPaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository)
        : IRequestHandler<CancelPaymentRequestCommand, Result<CancelPaymentRequestResponseViewModel>>
    {
        public async Task<Result<CancelPaymentRequestResponseViewModel>> Handle(CancelPaymentRequestCommand request, CancellationToken cancellationToken)
        {
            CancelPaymentRequestResponseViewModel url = new();
            try
            {
                PaymentRequest paymentRequest = await paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByCode(request.ViewModel.PaymentCode));
                if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();
                if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
                if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
                if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

                PaymentRequest.UpdateStatus(paymentRequest, Enums.PaymentStatus.CanceledByUser);
                await paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
                await paymentRequestRepository.SaveChangesAsync(cancellationToken);

                url.CallbackUrl = Constants.CreateCallbackUrl(paymentRequest.CallBackUrl, paymentRequest.PaymentCode, Enums.PaymentStatus.CanceledByUser);

                return Result<CancelPaymentRequestResponseViewModel>.SuccessResult(url);
            }
            catch (DomainException exc)
            {
                return Result<CancelPaymentRequestResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
            }
            catch (AppException exc)
            {
                return Result<CancelPaymentRequestResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
            }
            catch (Exception exc)
            {
                return Result<CancelPaymentRequestResponseViewModel>.Failure(new Error(exc.Source, exc.Message));
            }
        }
    }
}
