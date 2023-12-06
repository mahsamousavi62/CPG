using CPG.Application.UseCases.IPGType.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.IPGType.Queries;

public class GetIPGTypeQuery(long ipgTypeId) : IRequest<IPGTypeViewModel>
{
    public long IPGTypeId { get; } = ipgTypeId;
}