using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public record CreateCompanyIPGCommand(CreateCompanyIPGViewModel Model) : IRequest<Result<long>>;

