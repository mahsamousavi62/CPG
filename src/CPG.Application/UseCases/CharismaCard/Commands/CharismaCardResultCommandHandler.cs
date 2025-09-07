using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using CPG.Domain.SharedKernel.Interfaces;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CharismaCardResultCommandHandler(ICharismaCardService charismaCard,
											 IAggregateRepository<Transaction> transactionRepository,
											 IAggregateRepository<PaymentRequest> paymentRequestRepository) : IRequestHandler<CharismaCardResultCommand, Result<Unit>>
{
	public async Task<Result<Unit>> Handle(CharismaCardResultCommand request, CancellationToken cancellationToken)
	{

		if (string.IsNullOrWhiteSpace(request.TrackerId))
		{
			return Result<Unit>.Failure(new Error("", GlobalResource.TrackerIdEmpty));
		}

		Transaction transaction = await transactionRepository.FirstOrDefaultAsync(new TransactionByCharismaCardTrackId(request.trackerId), cancellationToken);

		if (transaction == null)
		{
			return Result<Unit>.Failure(new Error("2453002", GlobalResource.TrackerIdIsInvalid));
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
				paymentRequest.Status = Enums.PaymentStatus.TransactionWaitingForVerification;
				transaction.Status = TransactionStatus.InPrgress;
				transaction.PredictedSettlementDateTime = transaction.CharismaCardTransaction.CreationDate;
				break;
				case CharismaCardStatus.Failed:
				paymentRequest.Status = Enums.PaymentStatus.TransactionFailed;
				transaction.Status = TransactionStatus.TransactionFailed;
				break;
				default:
				break;
			}
			paymentRequest.ModificationDate = DateTime.Now;
			PaymentRequest.Update(paymentRequest);
			await paymentRequestRepository.UpdateAsync(paymentRequest);
			await paymentRequestRepository.SaveChangesAsync();
			await transactionRepository.UpdateAsync(transaction);
			await transactionRepository.SaveChangesAsync();

			return Result<Unit>.SuccessResult(Unit.Value);
		}
		else
		{
			return Result<Unit>.Failure(new Error(directDebitResultResponse.Error.Code, directDebitResultResponse.Error.Description));
		}

	}
}
