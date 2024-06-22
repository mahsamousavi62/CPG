using CPG.Application.Shared;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.PaymentRequests.Queries.Report;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.Redis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentRequests;

public class GetPaymentRequestsQueryHandler(ReadDbContext context,
                                            IRedisCacheService cacheService,
                                            IHttpContextAccessor httpContext,
                                            IAuthenticationService authenticationService,
                                            IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository)
                                            : IRequestHandler<GetPaymentRequestsQuery,
                                                              Result<PagedList<PaymentRequestReportViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IRedisCacheService _cacheService = cacheService;
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;

    public async Task<Result<PagedList<PaymentRequestReportViewModel>>> Handle(GetPaymentRequestsQuery request,
                                                                       CancellationToken cancellationToken)
    {
        try
        {
            var clientId = await _authenticationService.GetClientId();

            var application = await _applicationRepository.FirstOrDefaultAsync
                                                         (new ApplicationByIdpClientId(clientId), cancellationToken);

            Domain.AggregateModels.ApplicationAggregate.Application.Validate(application);

            Dictionary<long, string> usersCacheDictionary = await GetUsers();

            MappingConfig.RegisterMappings(usersCacheDictionary);

            var paymentRequests = _context.PaymentRequestReadModels.Include(c => c.Company)
                                                                   .ThenInclude(c => c.CompanyDeposits)
                                                                   .Where(p => p.ApplicationId == application.Id)
                                                                   .AsQueryable();

            PaymentRequestReadModel.AddFilter(request.PaymentFilter,ref paymentRequests);

            PaymentRequestReadModel.AddSort(request.PagedFilter, paymentRequests);

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
}
