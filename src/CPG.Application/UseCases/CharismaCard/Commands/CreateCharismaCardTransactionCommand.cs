
using CPG.Application.UseCases.CharismaCard.ViewModels;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CreateCharismaCardTransactionCommand(CharismaCardRequsetViewModel model) : IRequest<Result<CharismaCardResponseViewModel>>
{
    public CharismaCardRequsetViewModel Model { get; set; } = model;
}
