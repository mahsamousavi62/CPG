using Ardalis.Specification;
using Daryaftyar.Domain.SeedWork;

namespace Daryaftyar.Domain.SharedKernel
{
    public interface IAggregateRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}