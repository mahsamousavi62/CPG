using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.UpdateCompanyDeposit;

public class UpdateCompanyDepositCommandHandler(IAggregateRepository<CompanyDeposit> companyDepositRepository)
    : IRequestHandler<UpdateCompanyDepositCommand, Result<Unit>>
{
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;

    public async Task<Result<Unit>> Handle(UpdateCompanyDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (companyDeposit, persianName) = await Validate(request.Model);

            CompanyDeposit.Update(companyDeposit, persianName);

            await _companyDepositRepository.UpdateAsync(companyDeposit, cancellationToken);
            await _companyDepositRepository.SaveChangesAsync(cancellationToken);
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
    private async Task<(CompanyDeposit, PersianName)> Validate(UpdateCompanyDepositViewModel model)
    {
        PersianName persianName = new(model.Name);

        var companyDeposit = await _companyDepositRepository.GetByIdAsync(model.Id);
        if (companyDeposit == null)
            throw new CompanyDepositNotFoundException(model.Id);

        var samePersianName = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByNameForUpdateMode(model.Name, model.Id));
        if (samePersianName != null)
            throw new DuplicatCompanyDepositPersianNameException(model.Name);

        return (companyDeposit, persianName);
    }
}
