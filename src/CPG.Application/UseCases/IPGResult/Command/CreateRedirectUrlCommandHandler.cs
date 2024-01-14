using Ardalis.Specification;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users.Commands;

public class CreateRedirectUrlCommandHandler(IApplicationSettingsRepository applicationSettingsRepository, IAggregateRepository<Transaction> transactionRepository) : IRequestHandler<CreateRedirectUrlCommnad, Result<string>>
{
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;

    public async Task<Result<string>> Handle(CreateRedirectUrlCommnad command, CancellationToken cancellationToken)    
    {
        try
        {
            var appConfig = await _applicationSettingsRepository.GetAllApplicationSettings();

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByIPGTrackId(command.id));

            var url = string.Empty;
            switch (transaction.IPGTransaction.CompanyIPG.Provider.ProviderType)
            {
                case Enums.ProviderType.Vandar:
                    break;
                case Enums.ProviderType.AsanPardakht:
                    {
                        url = $"{appConfig.IPG_Callback_URL}?trackId={command.id}";
                        break;
                    }
                case Enums.ProviderType.Sep:
                    {
                        url = $"{appConfig.IPG_Callback_URL}?trackId={command.id}&MID={command.model.MID}&TerminalId={command.model.TerminalId}" +
                            $"&RefNum={command.model.RefNum}&ResNum={command.model.ResNum}&State={command.model.State}&TraceNo={command.model.TraceNo}" +
                            $"&Amount={command.model.Amount}&Wage={command.model.Wage}&Rrn={command.model.Rrn}&SecurePan={command.model.SecurePan}&" +
                            $"Token={command.model.Token}&HashedCardNumber={command.model.HashedCardNumber}&Status={command.model.Status}";
                        break;
                    }
                default:
                    break;
            }

            return Result<string>.SuccessResult(url);
        }
        catch (DomainException exc)
        {
            return Result<string>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<string>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<string>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}
