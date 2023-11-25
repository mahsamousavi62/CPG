using MediatR;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(CreateCompanyViewModel Model) : IRequest<long>;