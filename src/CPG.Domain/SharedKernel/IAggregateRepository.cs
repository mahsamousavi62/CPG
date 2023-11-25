using Ardalis.Specification;
using CPG.Domain.SeedWork;

namespace CPG.Domain.SharedKernel;

public interface IAggregateRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}