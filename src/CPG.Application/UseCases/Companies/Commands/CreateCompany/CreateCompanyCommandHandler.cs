using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler(IAggregateRepository<Company> companyRepository) : IRequestHandler<CreateCompanyCommand, long>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;

    public async Task<long> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        Logo logo = new(request.Model.File);

        var company = Company.Create(persianName, englishName,
                                    request.Model.NationalCodeMatchingRequied,
                                    logo, request.Model.MethodTypes);

        await _companyRepository.AddAsync(company, cancellationToken);
        await _companyRepository.SaveChangesAsync(cancellationToken);

        return company.Id;
    }
}