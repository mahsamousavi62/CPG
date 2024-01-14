using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Application.Queries;

public class GetApplicationQuery(long appId) : IRequest<Result<ApplicationViewModel>>
{
    public long AppId { get; } = appId;
}