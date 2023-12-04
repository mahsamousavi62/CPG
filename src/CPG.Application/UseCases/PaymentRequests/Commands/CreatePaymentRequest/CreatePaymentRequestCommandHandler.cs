using Ardalis.GuardClauses;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository, IAggregateRepository<Company> companyRepository) : IRequestHandler<CreatePaymentRequestCommand, PaymentRequestViewModel>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;

    public async Task<PaymentRequestViewModel> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        await Validate(request.Model);
        //var applicationId = 1;
        //var paymentRequest = PaymentRequest.Create(request.Model.CompanyId,
        //    request.Model.DestinationIban,
        //    applicationId,
        //    request.Model.NationalCode,
        //    request.Model.Description,
        //    request.Model.Amount,

        //    ,);
        //await _paymentRequestRepository.AddAsync(paymentRequest);
        await _paymentRequestRepository.SaveChangesAsync();
        throw new Exception();
    }

    private async Task Validate(CreatePaymentRequestViewModel model)
    {
        //TODO:check applicationId
        //TODO:check callbackUrl
       
        if (string.IsNullOrEmpty(model.DestinationIban))
        {
            var iban = new Iban(model.DestinationIban);
        }
        var amount = new Amount(model.Amount);
        var callBackUrl=new CallBackUrl(model.CallBackUrl);

        var nationalCode = new NationalCode(model.NationalCode);

        var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(model.DestinationIban));
        if (companyDeposit.CompanyId != model.CompanyId)
            throw new Exception();


        if (model.CompanyId.HasValue)
        {
            var comapny = await _companyRepository.GetByIdAsync(model.CompanyId);
            if (comapny != null) throw new Exception();
        }
        else
        {
        model.CompanyId = companyDeposit.CompanyId;
            
        }

        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new Exception();

    }
}
