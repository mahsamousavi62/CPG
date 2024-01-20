using Ardalis.Specification;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using MediatR;
using System;
using System.Text.Encodings.Web;
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
                        var encoder = UrlEncoder.Create();

                        url = $"{appConfig.IPG_Callback_URL}?trackId={encoder.Encode(command.id)}&MID={encoder.Encode(command.model.MID ?? "")}&TerminalId={encoder.Encode(command.model.TerminalId.ToString())}" +
                            $"&RefNum={encoder.Encode(command.model.RefNum ?? "")}&ResNum={encoder.Encode(command.model.ResNum ?? "")}&State={encoder.Encode(command.model.State ?? "")}&TraceNo={encoder.Encode(command.model.TraceNo ?? "")}" +
                            $"&Amount={encoder.Encode(command.model.Amount.ToString())}&Wage={encoder.Encode(command.model.Wage ?? "")}&Rrn={encoder.Encode(command.model.Rrn ?? "")}&SecurePan={encoder.Encode(command.model.SecurePan ?? "")}&" +
                            $"Token={encoder.Encode(command.model.Token ?? "")}&HashedCardNumber={encoder.Encode(command.model.HashedCardNumber ?? "")}&Status={encoder.Encode(command.model.Status.ToString())}";                        
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
