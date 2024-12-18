using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.UpdateCompanyDeposit;

public class UpdateCompanyDepositCommandHandler(IAggregateRepository<CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Company> companyRepository)
    : IRequestHandler<UpdateCompanyDepositCommand, Result<Unit>>
{
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;

    public async Task<Result<Unit>> Handle(UpdateCompanyDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (companyDeposit, persianName) = await Validate(request.Model, cancellationToken);

            CompanyDeposit.Update(companyDeposit, persianName, request.Model.methodTypes);

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
    private async Task<(CompanyDeposit, PersianName)> Validate(UpdateCompanyDepositViewModel model, CancellationToken cancellationToken)
    {
        PersianName persianName = new(model.Name);

        var companyDeposit = await _companyDepositRepository.GetByIdAsync(model.Id);
        if (companyDeposit == null)
            throw new CompanyDepositNotFoundException(model.Id);

        var samePersianName = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByNameForUpdateMode(model.Name, model.Id));
        if (samePersianName != null)
            throw new DuplicatCompanyDepositPersianNameException(model.Name);

        var company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsDataByIdSpec(companyDeposit.CompanyId),
           cancellationToken) ?? throw new CompanyNotFoundException(companyDeposit.CompanyId);

        if (companyDeposit.PaymentMethods.Any(t => !company.PaymentMethods.Select(x => x.MethodType).ToList().Contains(t.MethodType)))
        {
            throw new MethodTypeNotAllowedException(string.Empty);
        }

        return (companyDeposit, persianName);
    }
}
