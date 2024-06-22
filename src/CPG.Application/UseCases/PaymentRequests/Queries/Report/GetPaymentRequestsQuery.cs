using CPG.Application.Shared;

namespace CPG.Application.UseCases.PaymentRequests.Queries.Report;


public record GetPaymentRequestsQuery(PaymentFilter PaymentFilter,
                                     PagedFilter PagedFilter)
                                        : IRequest<Result<PagedList<PaymentRequestReportViewModel>>>;
