using CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.UpdateCompanyIPG;

public class UpdateCompanyIPGCommand(UpdateCompanyIPGViewModel model) : IRequest<Result<Unit>>
{
    public UpdateCompanyIPGViewModel Model { get; set; } = model;
}
