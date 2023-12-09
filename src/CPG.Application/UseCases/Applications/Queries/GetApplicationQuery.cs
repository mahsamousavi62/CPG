using CPG.Application.UseCases.Application.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Application.Queries;

public class GetApplicationQuery(long appId) : IRequest<ApplicationViewModel>
{
    public long AppId { get; } = appId;
}