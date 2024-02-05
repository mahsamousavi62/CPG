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

namespace CPG.Application.UseCases.CompanyDeposits.Commands.SetAsDefaultForDD;

public class SetAsDefaultForDDCommandHandler(IAggregateRepository<Company> companyRepository, WriteDbContext context)
    : IRequestHandler<SetAsDefaultForDDCommand, Result<bool>>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly WriteDbContext _context = context;

    public async Task<Result<bool>> Handle(SetAsDefaultForDDCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(request.model.CompanyId)) ?? throw new CompanyNotFoundException(request.model.CompanyId);

        var companyDeposit = company.CompanyDeposits?.Where(t => t.Id == request.model.CompanyDepositId).FirstOrDefault();

        if (companyDeposit is null)
        {
            throw new CompanyDepositNotBelongToCompanyException(companyDeposit.Id);
        }

        company.CompanyDeposits.ForEach(t => t.IsDefaultForDirectDebit = false);
        companyDeposit.IsDefaultForDirectDebit = true;

        _context.UpdateRange(company.CompanyDeposits);
        await _context.SaveChangesAsync();

        return Result<bool>.SuccessResult(true);
    }
}