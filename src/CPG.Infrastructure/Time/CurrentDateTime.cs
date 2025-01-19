using System;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Infrastructure.Time;

public class CurrentDateTime : ICurrentDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}