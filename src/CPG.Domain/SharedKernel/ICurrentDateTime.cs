using System;

namespace CPG.Domain.SharedKernel;

public interface ICurrentDateTime
{
    DateTime UtcNow { get; }
}