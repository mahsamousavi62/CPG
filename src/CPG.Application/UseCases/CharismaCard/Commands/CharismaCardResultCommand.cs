namespace CPG.Application.UseCases.CharismaCard.Commands;

public record CharismaCardResultCommand(string trackerId) : IRequest<Result<Unit>>
{
	public string TrackerId { get; set; } = trackerId;
}