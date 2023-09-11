using Ardalis.Specification;
using Daryaftyar.Domain.SeedWork;

namespace Daryaftyar.Domain.SharedKernel
{
    public interface IAggregateReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}