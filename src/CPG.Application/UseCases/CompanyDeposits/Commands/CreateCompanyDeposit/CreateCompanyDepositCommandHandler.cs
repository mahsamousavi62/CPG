using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;
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
        var (persianName, iban) = await Validate(request.Model);
        var bankId = await GetBankId(iban);
        var companyDeposit = CompanyDeposit.Create(persianName, iban, bankId,
                                request.Model.AccountNumber, request.Model.CompanyId);

        await _companyDepositRepository.AddAsync(companyDeposit, cancellationToken);
        await _companyDepositRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.SuccessResult(companyDeposit.Id);
    }

    private async Task<(PersianName, Iban)> Validate(CreateCompanyDepositViewModel companyDeposit)
    {
        PersianName persianName = new(companyDeposit.Name);
        Iban iban = new(companyDeposit.Iban);
        var sameIban = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(companyDeposit.Iban));
        if (sameIban != null)
            throw new DuplicatCompanyDepositIbanException(companyDeposit.Iban);

        var company = await _companyRepository.GetByIdAsync(companyDeposit.CompanyId) ?? throw new CompanyNotFoundException(companyDeposit.CompanyId);

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