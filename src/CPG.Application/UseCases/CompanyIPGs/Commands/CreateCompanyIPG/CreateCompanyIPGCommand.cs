using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public record CreateCompanyIPGCommand(CreateCompanyIPGViewModel Model) : IRequest<long>;

