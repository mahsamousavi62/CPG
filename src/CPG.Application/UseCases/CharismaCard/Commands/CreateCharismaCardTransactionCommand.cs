
using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CreateCharismaCardTransactionCommand(CharismaCardRequsetViewModel model) : IRequest<Result<CharismaCardResponseViewModel>>
{
    public CharismaCardRequsetViewModel Model { get; set; } = model;
}
