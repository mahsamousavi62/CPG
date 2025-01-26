using System;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface ICurrentDateTime
{
    DateTime UtcNow { get; }
}