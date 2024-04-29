using CPG.Application.Shared.Exceptions;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler(IAggregateRepository<Company> companyRepository,
                                            IAggregateRepository<User> userRepository,
                                            IMinioProvider minioProvider) : IRequestHandler<CreateCompanyCommand, Result<long>>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<User> _userRepository = userRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<long>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        await CheckUniqueName(request.Model.PersianName, request.Model.EnglishName);
        await CheckUniqueCode(request.Model.Code);
        Url siteAddress = new Url(request.Model.SiteAddress);

        if (!Enum.TryParse<Enums.IpgRedirectionMethodType>
            (request.Model.IpgRedirectionMethodType.ToString(), out Enums.IpgRedirectionMethodType methodType))
            throw new IpgRedirectionMethodTypeNotFoundException();

        Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Company.ToString(), _minioProvider);
        var spec = new UserByUserIdsSpec(request.Model.Users);
        var users = await userRepository.ListAsync(spec, cancellationToken);

        if (users == null || users.Count == 0)
            throw new UsersNotFoundException();

        if (request.Model.NationalCodeMatchingRequied is true &&
            request.Model.MethodTypes.ToList().Contains(Enums.PaymentMethodType.InternetPaymentGateway) &&
            (string.IsNullOrEmpty(request.Model.Key) || string.IsNullOrEmpty(request.Model.IV) || request.Model.ThirdPartyCode is null))
        {
            throw new RequiredShaparakSettingsException();
        }

        var company = Company.Create(persianName, englishName, request.Model.NationalCodeMatchingRequied, logo, request.Model.MethodTypes,
            siteAddress, request.Model.IpgRedirectionMethodType, request.Model.Key, request.Model.IV, request.Model.ThirdPartyCode, 
            request.Model.Code);

        await _companyRepository.AddAsync(company, cancellationToken);
        await _companyRepository.SaveChangesAsync(cancellationToken);

        User.UpdateUserCompany(users, company.Id);
        await _userRepository.UpdateRangeAsync(users, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.SuccessResult(company.Id);
    }

    private async Task CheckUniqueName(string persianName, string englishName)
    {
        Company samePersianName = await _companyRepository.FirstOrDefaultAsync(new CompanyByPersianName(persianName));

        if (samePersianName != null) throw new DuplicatePersianNameException(persianName);

        Company sameEnglishName = await _companyRepository.FirstOrDefaultAsync(new CompanyByEnglishName(englishName));

        if (sameEnglishName != null) throw new DuplicateEnglishNameException(englishName);
    }

    private async Task CheckUniqueCode(short code)
    {
        Company sameCode = await _companyRepository.FirstOrDefaultAsync(new CompanyByCodeSpec(code));

        if (sameCode != null) throw new DuplicateCodeException(code);
    }
}