using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Queries;

public record GetAllCompanyQuery : IRequest<Result<IReadOnlyCollection<CompanyViewModel>>>;