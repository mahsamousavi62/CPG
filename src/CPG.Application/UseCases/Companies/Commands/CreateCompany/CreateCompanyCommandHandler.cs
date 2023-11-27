using CPG.Application.UseCases.Files.Commands.UploadFile;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler(IAggregateRepository<Company> companyRepository,IAggregateRepository<User> userRepository,
                                         IMinioProvider minioProvider): IRequestHandler<CreateCompanyCommand, long>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<User> _userRepository = userRepository;
    private readonly IMinioProvider _minioProvider=minioProvider;

    public async Task<long> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        Logo logo = new(request.Model.File,Enums.UploadFromEntityType.Company.ToString(), _minioProvider);

       
        var spec = new UserByUserIdsSpec(request.Model.Users);
        var users = await userRepository.ListAsync(spec, cancellationToken);

        var company = Company.Create(persianName, englishName,
                                    request.Model.NationalCodeMatchingRequied,
                                    logo, request.Model.MethodTypes);

        await _companyRepository.AddAsync(company, cancellationToken);
        await _companyRepository.SaveChangesAsync(cancellationToken);

        User.UpdateUserCompany(users, company.Id);
        await _userRepository.UpdateRangeAsync(users,cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
       
            return company.Id;
    }
}