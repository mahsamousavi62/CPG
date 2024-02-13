using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetUserPhoneNumbersCommand(GetUserPhoneNumbersViewModel model) : IRequest<Result<IReadOnlyCollection<UserPhoneNumberViewModel>>>
{
    public GetUserPhoneNumbersViewModel model { get; set; } = model;
}