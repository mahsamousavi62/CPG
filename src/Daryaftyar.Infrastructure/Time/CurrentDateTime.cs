using System;
using Daryaftyar.Domain.SharedKernel;

namespace Daryaftyar.Infrastructure.Time
{
    public class CurrentDateTime : ICurrentDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}