using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyPaymentMethodsQuery : IRequest<Dictionary<int, string>>
{
}

