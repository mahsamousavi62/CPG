using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;
using System;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;

namespace CPG.Application.UseCases.Banks.Commands.UpdateBank;

public class UpdateBankCommandHandler(IAggregateRepository<Bank> bankRepository, ICurrentUser currentUser) : IRequestHandler<UpdateBankCommand, Result<bool>>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;

    public async Task<Result<bool>> Handle(UpdateBankCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var model = command.model;
            var bank = await _bankRepository.GetBySpecAsync(new BankByIdSpec(model.BankId), cancellationToken)
                       ?? throw new BankNotFoundException(model.BankId);

            bank.Update(model.Name, model.LogoAddress, model.IbanPrefix, model.HasDirectDebitFeature, model.DirectDebitSetting.ProviderId,
                model.DirectDebitSetting.DDBankCode, model.DirectDebitSetting.MaxWithdrawalAmountPerDay,
                model.DirectDebitSetting.MaxMandateValidityDurationPerMonth, model.DirectDebitSetting.AuthenticationType);

            await _bankRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error(ex.Source, ex.Message));
        }
    }
}
