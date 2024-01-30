using MediatR;

namespace CPG.Application.UseCases.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(UpdateCompanyViewModel Model) : IRequest<Unit>;
