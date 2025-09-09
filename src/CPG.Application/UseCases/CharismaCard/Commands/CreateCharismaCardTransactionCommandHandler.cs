using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel.Interfaces;
using static CPG.Domain.SharedKernel.Enums;
using Transaction = CPG.Domain.AggregateModels.TransactionAggregate.Transaction;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CreateCharismaCardTransactionCommandHandler(
   ICharismaCardService charismaCardService,
	IAuthenticationService authenticationService,
	IAggregateRepository<Transaction> transactionRepository,
	IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
	IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
	IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
	IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository,
	IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository
,
	IApplicationSettingsRepository applicationSettingsRepository)
	: IRequestHandler<CreateCharismaCardTransactionCommand, Result<CharismaCardResponseViewModel>>
{
	private readonly IApplicationSettingsRepository applicationSettingsRepository = applicationSettingsRepository;
	private string notGrantForDirectDebitError = "10";
	private readonly ICharismaCardService charismaCardService = charismaCardService;
	private readonly IAuthenticationService _authenticationService = authenticationService;
	private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
	private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
	private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
	private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
	private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;
	private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
	public async Task<Result<CharismaCardResponseViewModel>> Handle(CreateCharismaCardTransactionCommand request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Model.SourceIban))
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error("", GlobalResource.SourceIbanEmpty));
		}

		var iban = new Iban(request.Model.SourceIban);
		var appConfig = await applicationSettingsRepository.GetAllApplicationSettings();

		var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByCode(request.Model.PaymentCode), cancellationToken);

		PaymentRequest.Validate(paymentRequest);

		if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

		var company = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);

		long destinationDepositId;
		Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit companyDeposit;

		companyDeposit = await _companyDepositRepository.FirstOrDefaultAsync(new DefaultCharismaCardDepositSpec(paymentRequest.CompanyId), cancellationToken);
		if (companyDeposit is null) throw new Exception("Default CompanyDeposit for charismCard not found!");

		if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
		var bank = companyDeposit.Bank;

		if (!bank.IsActive) { throw new PaymentTokenInactiveBankException(); }

		if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.CharismaCard) is false)
		{ throw new PaymentRequestCompanyHasNoCharismaCardMethodException(); }

		destinationDepositId = companyDeposit.Id;
		var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
		string redirectUrl = $"{appConfig.Charisma_Card_Callback_URL}?trackId={trackerId}";

		var clientDirectDebitResponse = await charismaCardService.DirectDebitRequest(new Domain.SharedKernel.Communication.CharismaCard.Models.DirectDebitRequest
		{
			Amount = paymentRequest.Amount,
			SourceIban = request.Model.SourceIban,
			DestinationIban = companyDeposit.Iban,
			NationalCode = paymentRequest.NationalCode,
			RedirectUrl = redirectUrl,
			TrackerId = trackerId,
		});


		DirectDebitResponse clientDirectDebit = clientDirectDebitResponse.Data;

		if (clientDirectDebitResponse?.IsSuccess == true && clientDirectDebitResponse.Data is not null)
		{
			var CharismaCardstatus = CharismaCardStatus.InProgress;
			Transaction transaction = Transaction.Create(new CreateTransactionModel
			{
				CharismaCardModel = new CharismaCardTransactionModel
				{
					TrackId = trackerId,
					SourceIban = request.Model.SourceIban,
					ProviderTrackId = Regex.Match(clientDirectDebitResponse.Data.Data.Url, @"(\d+)$").Value,
					Status = CharismaCardstatus
				},

				DestinationDepositId = destinationDepositId,
				PaymentRequest = paymentRequest,
				TransactionMethodType = Enums.TransactionType.CharismaCard,
				Status = Enums.TransactionStatus.InPrgress
			});

			paymentRequest.Status = Enums.PaymentStatus.InProgress;
			transaction.PredictedSettlementDateTime = transaction.CharismaCardTransaction.CreationDate;
			paymentRequest.ModificationDate = DateTime.Now;
			paymentRequest.IsUsed = true;

			PaymentRequest.Update(paymentRequest);
			await _paymentRequestRepository.UpdateAsync(paymentRequest);
			await _paymentRequestRepository.SaveChangesAsync();
			await _transactionRepository.UpdateAsync(transaction);
			await _transactionRepository.SaveChangesAsync();

			return Result<CharismaCardResponseViewModel>.SuccessResult(new CharismaCardResponseViewModel
			{
				CallBackUrl = clientDirectDebitResponse.Data.Data.Url
			});
		}
		else
		{
			return Result<CharismaCardResponseViewModel>.Failure(new Error(clientDirectDebitResponse.Error.Code, clientDirectDebitResponse.Error.Description));
		}
	}
}
