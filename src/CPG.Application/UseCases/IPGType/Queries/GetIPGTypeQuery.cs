using CPG.Application.UseCases.IPGTypes.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.IPGTypes.Queries;

public class GetIPGTypeQuery(long ipgTypeId) : IRequest<IPGTypeViewModel>
{
    public long IPGTypeId { get; } = ipgTypeId;
}