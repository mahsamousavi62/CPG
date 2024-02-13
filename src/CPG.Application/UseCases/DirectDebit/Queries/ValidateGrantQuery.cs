using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Queries;

public class ValidateGrantQuery(ValidateGrantRequestViewModel model, string trackId) : IRequest<Result<ValidateGrantResponseViewModel>>
{
    public ValidateGrantRequestViewModel ValidateGrant { get; set; } = model;

    public string TrackId { get; set; } = trackId;
}