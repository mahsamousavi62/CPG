using System;

namespace Daryaftyar.Domain.SharedKernel
{
    public interface ICurrentDateTime
    {
        DateTime UtcNow { get; }
    }
}