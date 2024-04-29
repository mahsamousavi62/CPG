using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public class CreateCompanyDepositCommandHandler(IAggregateRepository<CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<Bank> bankRepository) : IRequestHandler<CreateCompanyDepositCommand, Result<long>>
{
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;

    public async Task<Result<long>> Handle(CreateCompanyDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (persianName, iban) = await Validate(request.Model, cancellationToken);
            var bankId = await GetBankId(iban);

            var isFirstDeposit = (await _companyDepositRepository.GetBySpecAsync(new CompanyHasAnyDepositSpec(request.Model.CompanyId), cancellationToken)) != null ? true : false;
            var companyDeposit = CompanyDeposit.Create(persianName, iban, bankId, request.Model.AccountNumber, request.Model.CompanyId,
                isFirstDeposit, request.Model.MethodTypes);

            await _companyDepositRepository.AddAsync(companyDeposit, cancellationToken);
            await _companyDepositRepository.SaveChangesAsync(cancellationToken);

            return Result<long>.SuccessResult(companyDeposit.Id);
        }
        catch (DomainException exc)
        {
            return Result<long>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<long>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<long>.Failure(new Error(exc.Source, exc.Message));
        }
    }

    private async Task<(PersianName, Iban)> Validate(CreateCompanyDepositViewModel companyDeposit, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsDataByIdSpec(companyDeposit.CompanyId),
            cancellationToken) ?? throw new CompanyNotFoundException(companyDeposit.CompanyId);

        if (companyDeposit.MethodTypes.Any(t => !company.PaymentMethods.Select(x => x.MethodType).Contains(t)))
        {
            throw new MethodTypeNotAllowedException(string.Empty);
        }

        Iban iban = new(companyDeposit.Iban);
        var sameIban = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(companyDeposit.Iban));
        if (sameIban != null)
            throw new DuplicatCompanyDepositIbanException(companyDeposit.Iban);

        PersianName persianName = new(companyDeposit.Name);
        var samePersianName = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByName(companyDeposit.Name));
        if (samePersianName != null)
            throw new DuplicatCompanyDepositPersianNameException(companyDeposit.Name);


        return (persianName, iban);
    }

    private async Task<int> GetBankId(string iban)
    {
        var prefix = iban.Substring(4, 3);

        var bank = await _bankRepository.GetBySpecAsync(new BankByIbanPrefixSpec(prefix));
        if (!bank.IsActive)
        {
            throw new BankIsNotActiveException(bank.Id);
        }
        return bank.Id;
    }
}