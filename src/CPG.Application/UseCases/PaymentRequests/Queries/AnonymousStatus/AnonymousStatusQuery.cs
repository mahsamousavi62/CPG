using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.ViewModels;

namespace CPG.Application.UseCases.PaymentRequests.Queries.AnonymousStatus;

public class AnonymousStatusQuery(AnonymousStatusViewModel model) : IRequest<Result<AnonymousStatusResponseViewModel>>
{
    public AnonymousStatusViewModel Model { get; set; } = model;
}
