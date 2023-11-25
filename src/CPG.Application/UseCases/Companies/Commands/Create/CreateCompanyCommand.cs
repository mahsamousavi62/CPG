using MediatR;

namespace CPG.Application.UseCases.Companies.Commands.Create;

public class CreateCompanyCommand(CreateCompanyViewModel model) : IRequest<Unit>
{
    public CreateCompanyViewModel Model { get; set; } = model;
}
