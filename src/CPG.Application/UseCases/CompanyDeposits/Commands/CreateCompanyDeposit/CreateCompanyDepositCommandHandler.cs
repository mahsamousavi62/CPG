

using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public class CreateCompanyDepositCommandHandler(IAggregateRepository<CompanyDeposit> companyRepository): IRequestHandler<CreateCompanyDepositCommand, long>
{
    private readonly IAggregateRepository<CompanyDeposit> _companyRepository = companyRepository;

    public async Task<long> Handle(CreateCompanyDepositCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}