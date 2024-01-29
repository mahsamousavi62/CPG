using CPG.Application.UseCases.IPGResult.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateRedirectUrlCommnad(IFormCollection form, string id) : IRequest<Result<string>>
{
    public IFormCollection form=form;
    public string id=id;
    
}
