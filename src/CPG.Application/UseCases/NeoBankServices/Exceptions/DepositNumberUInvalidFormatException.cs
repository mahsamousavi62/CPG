using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Banks.Exceptions;

public class InvalidDepositNumberFormatException(string depositNumber) : AppException(string.Format(GlobalResource.InvalidDepositNumber, depositNumber))
{
    public override string Code => "invalid_depositNumber";
    public string DepositNumber { get; } = depositNumber;
}