using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.UpdateCompanyIPG;

public class UpdateCompanyIPGCommandHandler(IAggregateRepository<CompanyIPG> companyIPGRepository,
    IAggregateRepository<CompanyDeposit> depositRepository) : IRequestHandler<UpdateCompanyIPGCommand, Result<Unit>>
{
    private readonly IAggregateRepository<CompanyIPG> _aggregateRepository = companyIPGRepository;
    public IAggregateRepository<CompanyDeposit> _depositRepository { get; } = depositRepository;

    public async Task<Result<Unit>> Handle(UpdateCompanyIPGCommand request, CancellationToken cancellationToken)
    {
        try
        {
            CompanyIPG companyIpg = await Validate(request);

            var companyIPGDeposits = request.Model.CompanyIPGDeposits.Select(t => new CompanyIPGDeposit(t.DepositId, t.IsDefault)).ToArray();
            CompanyIPG.Update(companyIpg, request.Model.CompanyId,
                                                request.Model.ProviderId,
                                                request.Model.IPGTypeId,
                                                request.Model.ProviderData,
                                                companyIPGDeposits);

            await _aggregateRepository.UpdateAsync(companyIpg, cancellationToken);
            await _aggregateRepository.SaveChangesAsync(cancellationToken);

            return Result<Unit>.SuccessResult(Unit.Value);
        }
        catch (DomainException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<Unit>.Failure(new Error(exc.Source, exc.Message));
        }
    }

    private async Task<CompanyIPG> Validate(UpdateCompanyIPGCommand request)
    {
        var companyIpg = await _aggregateRepository.GetBySpecAsync(new CompanyIPGById(request.Model.Id));
            if (companyIpg is null)
            throw new CompanyIPGNotFoundException(request.Model.Id);

        var depositsIds = request.Model.CompanyIPGDeposits.Select(t => t.DepositId).ToList();
        var deposits = await _depositRepository.ListAsync(new CompanyDepositsByIdList(depositsIds));

        var notFoundDeposits = depositsIds.Where(t => !deposits.Select(d => d.Id).Contains(t));
        if (notFoundDeposits?.Any() is true)
            throw new NotFoundDepositException(string.Join(',', notFoundDeposits));

        var invalidDeposits = deposits.Where(t => t.CompanyId != request.Model.CompanyId).Select(t => t.Id);
        if (invalidDeposits?.Any() is true)
            throw new InvalidDepositsException(string.Join(',', invalidDeposits), request.Model.CompanyId);
        return companyIpg;
    }
}
