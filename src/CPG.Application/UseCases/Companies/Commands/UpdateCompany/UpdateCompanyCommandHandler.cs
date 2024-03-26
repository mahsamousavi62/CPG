using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using CPG.Application.Shared.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Application.UseCases.Application.Exceptions;
using System.Reflection;
using Ardalis.GuardClauses;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;

namespace CPG.Application.UseCases.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandHandler(IAggregateRepository<Company> companyRepository,IAggregateRepository<User> userRepository,
                                            IMinioProvider minioProvider) : IRequestHandler<UpdateCompanyCommand, Result<Unit>>
{
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<User> _userRepository = userRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<Unit>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {

        try
        {
            PersianName persianName = new(request.Model.PersianName);
            EnglishName englishName = new(request.Model.EnglishName);

            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec( request.Model.Id));
            if (company == null)
                throw new CompanyNotFoundException(request.Model.Id);

            await CheckUniqueName(company.Id, request.Model.PersianName, request.Model.EnglishName);
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

            Company.Update(company, persianName, englishName, request.Model.NationalCodeMatchingRequied, logo, request.Model.MethodTypes,
               siteAddress, request.Model.IpgRedirectionMethodType, request.Model.Key, request.Model.IV, request.Model.ThirdPartyCode);

            await _companyRepository.UpdateAsync(company, cancellationToken);
            await _companyRepository.SaveChangesAsync(cancellationToken);

            User.UpdateUserCompany(users, company.Id,company);
            await _userRepository.UpdateRangeAsync(users, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return Result<Unit>.SuccessResult(Unit.Value);
        }
        catch (DomainException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<Unit>.Failure(new Error(exc.Source, exc.Message));
        }


    }

    private async Task CheckUniqueName(long id, string persianName, string englishName)
    {
        Company samePersianName = await _companyRepository.GetBySpecAsync(new CompanyByPersianNameUpdateMode(persianName,id));

        if (samePersianName != null) throw new DuplicatePersianNameException(persianName);

        Company sameEnglishName = await _companyRepository.GetBySpecAsync(new CompanyByEnglishNameUpdateMode(englishName,id));

        if (sameEnglishName != null) throw new DuplicateEnglishNameException(englishName);
    }
}
