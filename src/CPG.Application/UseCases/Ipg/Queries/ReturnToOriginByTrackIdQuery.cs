
using CPG.Application.UseCases.Ipg.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;
public class ReturnToOriginByTrackIdQuery(ReturnToOriginByTrackIdViewModel model) : IRequest<string>
{
    public ReturnToOriginByTrackIdViewModel model = model;
}
