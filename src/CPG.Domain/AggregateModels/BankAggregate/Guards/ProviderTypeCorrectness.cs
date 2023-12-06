using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate.Guards;

public static partial class ProviderTypeCorrectnessExtension
{
    public static int ProviderTypeCorrectness(this IGuardClause guardClause, [MaybeNull] int input, string parameterName, string message = null)
    {
        if(!Enum.IsDefined(typeof(ProviderType), input))
            throw new ProviderTypeInvalidValueException(input);

        return input;
    }
}
