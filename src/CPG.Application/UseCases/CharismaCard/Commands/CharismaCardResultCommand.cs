using CPG.Application.UseCases.CharismaCard.ViewModels;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public record CharismaCardResultCommand(string trackerId) : IRequest<Result<CharismaCardResponseViewModel>>
{
	public string TrackerId { get; set; } = trackerId;
}