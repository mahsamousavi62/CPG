using CPG.Domain.SharedKernel;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class CreateCompanyIPGCommandHandler(IAggregateRepository<CompanyIPG> companyIPGRepository,
    IAggregateRepository<CompanyDeposit> depositRepository) : IRequestHandler<CreateCompanyIPGCommand, long>
{
    private readonly IAggregateRepository<CompanyIPG> _aggregateRepository = companyIPGRepository;
    private readonly IAggregateRepository<CompanyDeposit> _depositRepository = depositRepository;

    public async Task<long> Handle(CreateCompanyIPGCommand request, CancellationToken cancellationToken)
    {
        var depositsIds = request.Model.CompanyIPGDeposits.Select(t => t.DepositId).ToList();
        var deposits = await _depositRepository.ListAsync(new CompanyDepositsByIdList(depositsIds));
        var invalidDeposits = deposits.Where(t => t.CompanyId != request.Model.CompanyId).Select(t => t.Id);
        if (invalidDeposits?.Any() is true)
        {
            throw new InvalidDepositsException(string.Join('-', invalidDeposits), request.Model.CompanyId);
        }
        var companyIPGDeposits = request.Model.CompanyIPGDeposits.Select(t => new CompanyIPGDeposit(t.DepositId, t.IsDefault)).ToArray();
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