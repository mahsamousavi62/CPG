

using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public class CreateCompanyDepositCommandHandler(IAggregateRepository<CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<Bank> bankRepository) : IRequestHandler<CreateCompanyDepositCommand, long>
{
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;


    public async Task<long> Handle(CreateCompanyDepositCommand request, CancellationToken cancellationToken)
    {
        var bank = await _bankRepository.GetByIdAsync(request.Model.BankId);
        if (bank == null) throw new System.Exception();

        var company = await _companyRepository.GetByIdAsync(request.Model.CompanyId);
        if (company == null) throw new System.Exception();

        var companyDeposit = CompanyDeposit.Create(request.Model.Name, new Iban(request.Model.Iban),
                                     request.Model.BankId, request.Model.AccountNumber, request.Model.CompanyId);

        await _companyDepositRepository.AddAsync(companyDeposit, cancellationToken);
        await _companyDepositRepository.SaveChangesAsync(cancellationToken);

        return companyDeposit.Id;

    }
}