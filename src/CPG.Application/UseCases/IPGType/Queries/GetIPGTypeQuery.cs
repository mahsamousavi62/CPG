using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.IPGTypes.Queries;

public class GetIPGTypeQuery(long ipgTypeId) : IRequest<Result<IPGTypeViewModel>>
{
    public long IPGTypeId { get; } = ipgTypeId;
}