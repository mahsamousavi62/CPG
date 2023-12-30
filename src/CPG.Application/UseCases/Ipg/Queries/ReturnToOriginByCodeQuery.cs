using CPG.Application.UseCases.Ipg.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries
{
    public class ReturnToOriginByCodeQuery(ReturnToOriginByCodeViewModel model) : IRequest<string>
    {
        public ReturnToOriginByCodeViewModel model = model;
    }
}
