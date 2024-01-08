using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyPaymentMethodsQuery : IRequest<Result<Dictionary<int, string>>>;