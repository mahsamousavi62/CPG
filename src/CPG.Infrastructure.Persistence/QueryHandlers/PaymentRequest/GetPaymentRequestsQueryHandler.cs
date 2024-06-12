using CPG.Application.Shared;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using CPG.Infrastructure.Persistence.Redis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentRequests;

public class GetPaymentRequestsQueryHandler(ReadDbContext context,
                                            IMinioProvider minioProvider,
                                            IRedisCacheService cacheService,
                                            IHttpContextAccessor httpContext,
                                            IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository)
                                                                            : IRequestHandler<GetPaymentRequestsQuery,
                                                                               Result<PagedList<PaymentRequestReportViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly IRedisCacheService _cacheService = cacheService;
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;


    public async Task<Result<PagedList<PaymentRequestReportViewModel>>> Handle(GetPaymentRequestsQuery request,
                                                                       CancellationToken cancellationToken)
    {
        try
        {
            var clientId = _httpContext.HttpContext.User.Claims.FirstOrDefault
               (c => c.Type == "client_id")?.Value;

            var application = await _applicationRepository.FirstOrDefaultAsync(new ApplicationByIdpClientId(clientId), cancellationToken);
            if (application == null)
                throw new PaymentRequestApplicationNotFoundException();
            if (!application.IsActive)
                throw new PaymentRequestApplicationIsInactiveException(application.PersianName, application.EnglishName);

            Dictionary<long, string> usersCacheDictionary = await GetUsers();
            MappingConfig.RegisterMappings(usersCacheDictionary);

            var paymentRequests = _context.PaymentRequestReadModels.Include(c => c.Company).ThenInclude(c=>c.CompanyDeposits)
                                    .Where(p => p.ApplicationId == application.Id).AsQueryable();

            #region [ Filter ]
            if (!string.IsNullOrWhiteSpace(request.PaymentFilter.PaymentCode))
            {
                paymentRequests = paymentRequests.Where(p =>
                   p.PaymentCode == request.PaymentFilter.PaymentCode);
            }
            if (request.PaymentFilter.Amount.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                   p.Amount == request.PaymentFilter.Amount);
            }
            if (!string.IsNullOrEmpty(request.PaymentFilter.NationalCode))
            {
                paymentRequests = paymentRequests.Where(p =>
                   p.NationalCode == request.PaymentFilter.NationalCode);
            }
            if (!string.IsNullOrEmpty(request.PaymentFilter.DestinationDepositIban))
            {
                paymentRequests = paymentRequests.Where(p =>
                   p.DestinationDepositIban == request.PaymentFilter.DestinationDepositIban);
            }

            if (request.PaymentFilter.CompanyId.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                  p.CompanyId == request.PaymentFilter.CompanyId);
            }

            if (request.PaymentFilter.Status.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                  p.Status == request.PaymentFilter.Status);
            }
            if (!string.IsNullOrEmpty(request.PaymentFilter.TrackerId))
            {
                paymentRequests = paymentRequests.Where(p =>
                 p.TrackerId == request.PaymentFilter.TrackerId);
            }
            if (request.PaymentFilter.FromCreationDate.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
              p.CreationDate.Date >= request.PaymentFilter.FromCreationDate.Value.Date);
            }
            if (request.PaymentFilter.ToCreationDate.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                p.CreationDate.Date <= request.PaymentFilter.ToCreationDate.Value.Date);
            }

            if (request.PaymentFilter.FromModificationDate.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                p.ModificationDate.Value.Date >= request.PaymentFilter.FromModificationDate.Value.Date);
            }
            if (request.PaymentFilter.ToModificationDate.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                p.ModificationDate.Value.Date <= request.PaymentFilter.ToModificationDate.Value.Date);
            }

            if (request.PaymentFilter.FromUrlExpirationDateTime.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                p.UrlExpirationDateTime.Date >= request.PaymentFilter.FromUrlExpirationDateTime.Value.Date);
            }
            if (request.PaymentFilter.ToUrlExpirationDateTime.HasValue)
            {
                paymentRequests = paymentRequests.Where(p =>
                p.UrlExpirationDateTime.Date <= request.PaymentFilter.ToUrlExpirationDateTime.Value.Date);
            }

            #endregion

            #region [ Sort ]
            if (request.PagedFilter.SortOrder?.ToLower() == "desc")
            {
                paymentRequests = paymentRequests.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                paymentRequests = paymentRequests.OrderBy(GetSortProperty(request));
            }

            #endregion

            var paymentRequestsResponsesQuery = paymentRequests.ProjectToType<PaymentRequestReportViewModel>();

            var products = await PagedList<PaymentRequestReportViewModel>.CreateAsync(
                paymentRequestsResponsesQuery,
                request.PagedFilter.PageNumber.Value,
                request.PagedFilter.PageSize.Value);

            return Result<PagedList<PaymentRequestReportViewModel>>.SuccessResult(products);
        }
        catch (Exception)
        {
            return Result<PagedList<PaymentRequestReportViewModel>>.Failure(new Error("1008000", GlobalResource.UnexpectedError));
        }
    }

    private async Task<Dictionary<long, string>> GetUsers()
    {
        var userscacheData = _cacheService.GetData<List<UserReadModel>>("AllUser_key");

        if (userscacheData == null)
        {
            userscacheData = await _context.UserReadModels.ToListAsync();
            _cacheService.SetData("AllUser_key", userscacheData);
        }


        var usersCacheDictionary = userscacheData.ToDictionary(c => c.Id,
                                                               c => $"{c.FirstName}  {c.LastName}");
        return usersCacheDictionary;
    }

    private static Expression<Func<PaymentRequestReadModel, object>> GetSortProperty(GetPaymentRequestsQuery request) =>
           request.PagedFilter.SortColumn?.ToLower() switch
           {
               "amount" => product => product.Amount,
               "creationdate" => product => product.CreationDate,
               "modificationdate" => product => product.ModificationDate,
               _ => product => product.Id
           };
}
