using CPG.Domain.SharedKernel;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class CreateCompanyIPGCommandHandler(IAggregateRepository<CompanyIPG> aggregateRepository)
    : IRequestHandler<CreateCompanyIPGCommand, long>
{
    private readonly IAggregateRepository<CompanyIPG> _aggregateRepository = aggregateRepository;

    public async Task<long> Handle(CreateCompanyIPGCommand request, CancellationToken cancellationToken)
    {
        var companyIPGDeposits = request.Model.CompanyIPGDeposits.Select(t => new CompanyIPGDeposit(t.CompanyDepositId,t.IsDefault)).ToArray();
        var companyIPG = CompanyIPG.Create(request.Model.CompanyId,
                                           request.Model.ProviderId,
                                           request.Model.IPGTypeId,
                                           request.Model.VerificationTimeLimit,
                                           request.Model.ProviderData,
                                           companyIPGDeposits);

        await _aggregateRepository.AddAsync(companyIPG, cancellationToken);
        await _aggregateRepository.SaveChangesAsync(cancellationToken);

        return companyIPG.Id;
    }
}