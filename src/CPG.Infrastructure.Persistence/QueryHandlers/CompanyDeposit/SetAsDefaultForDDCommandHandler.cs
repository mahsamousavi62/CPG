using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CPG.Infrastructure.Persistence.DbContexts;
using System.Linq;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.SetAsDefaultForDD;

public class SetAsDefaultForDDCommandHandler(IAggregateRepository<Company> companyRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository,WriteDbContext context)
    : IRequestHandler<SetAsDefaultForDDCommand, Result<bool>>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly WriteDbContext _context = context;

    public async Task<Result<bool>> Handle(SetAsDefaultForDDCommand request, CancellationToken cancellationToken)
    {
        var companyDeposit = await _companyDepositRepository.GetByIdAsync(request.model.CompanyDepositId) ?? throw new CompanyDepositNotFoundException(request.model.CompanyDepositId);

        var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(companyDeposit.CompanyId)) ;


        company.CompanyDeposits.ForEach(t => t.IsDefaultForDirectDebit = false);
        companyDeposit.IsDefaultForDirectDebit = true;

        _context.UpdateRange(company.CompanyDeposits);
        await _context.SaveChangesAsync();

        return Result<bool>.SuccessResult(true);
    }
}