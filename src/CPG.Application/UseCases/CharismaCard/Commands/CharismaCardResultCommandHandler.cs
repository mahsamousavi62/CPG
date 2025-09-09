using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using CPG.Domain.SharedKernel.Interfaces;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CharismaCardResultCommandHandler(ICharismaCardService charismaCard,
											 IAggregateRepository<Transaction> transactionRepository,
											 IAggregateRepository<PaymentRequest> paymentRequestRepository) : IRequestHandler<CharismaCardResultCommand, Result<CharismaCardResponseViewModel>>
{
	public async Task<Result<CharismaCardResponseViewModel>> Handle(CharismaCardResultCommand request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.TrackerId))
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error("", GlobalResource.TrackerIdEmpty));
		}

		Transaction transaction = await transactionRepository.FirstOrDefaultAsync(new TransactionByCharismaCardTrackId(request.trackerId), cancellationToken);

		if (transaction == null)
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error("2453002", GlobalResource.TrackerIdIsInvalid));
		}

		Result<DirectDebitResultResponse> directDebitResultResponse = await charismaCard.DirectDebitInquiry(new DirectDebitResultRequest { TrackerId = request.trackerId });

		if (directDebitResultResponse?.IsSuccess == true && directDebitResultResponse.Data.Data is not null)
		{
			DirectDebitResultResponse clientDirectDebit = directDebitResultResponse.Data;

			CharismaCardStatus charismaCardstatus = clientDirectDebit.Data.Status == 1 ? CharismaCardStatus.Done : CharismaCardStatus.Failed;
			transaction.CharismaCardTransaction.ModificationDate = DateTime.Now;
			transaction.CharismaCardTransaction.ReferenceNumber = request.trackerId;
			transaction.CharismaCardTransaction.Status = charismaCardstatus;

			transaction.Status = charismaCardstatus == CharismaCardStatus.Done ? Enums.TransactionStatus.InPrgress : Enums.TransactionStatus.TransactionFailed;
			transaction.ModificationDate = DateTime.Now;

			PaymentRequest paymentRequest = await paymentRequestRepository.GetByIdAsync(transaction.PaymentRquestId, cancellationToken);

			switch (charismaCardstatus)
			{
				case CharismaCardStatus.Done:
				{
					paymentRequest.Status = Enums.PaymentStatus.TransactionWaitingForVerification;
					transaction.Status = TransactionStatus.InPrgress;
					transaction.PredictedSettlementDateTime = transaction.CharismaCardTransaction.CreationDate;
					break;
				}
				case CharismaCardStatus.Failed:
				{
					paymentRequest.Status = Enums.PaymentStatus.TransactionFailed;
					transaction.Status = TransactionStatus.TransactionFailed;
					break;
				}
				default:
				break;
			}
			paymentRequest.ModificationDate = DateTime.Now;
			PaymentRequest.Update(paymentRequest);
			await paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
			await paymentRequestRepository.SaveChangesAsync(cancellationToken);
			await transactionRepository.UpdateAsync(transaction, cancellationToken);
			await transactionRepository.SaveChangesAsync(cancellationToken);

			return Result<CharismaCardResponseViewModel>.SuccessResult(new CharismaCardResponseViewModel
			{
				CallBackUrl = Constants.CreateCallbackUrl(transaction.PaymentRequest.CallBackUrl, transaction.PaymentRequest.PaymentCode, transaction.PaymentRequest.Status)
			});
		}
		else if (directDebitResultResponse?.IsSuccess == true && directDebitResultResponse.Data.Data is  null)
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error("2453003", GlobalResource.CharismaCardHasNotTrackerId));
		}
		else
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error(directDebitResultResponse.Error.Code, directDebitResultResponse.Error.Description));
		}
	}
}