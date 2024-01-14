using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Queries;

public record GetPaymentRequestQuery : IRequest<Result<IReadOnlyCollection<PaymentRequestViewModel>>>;