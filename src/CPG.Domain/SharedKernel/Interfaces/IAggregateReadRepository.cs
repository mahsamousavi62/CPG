using Ardalis.Specification;
using CPG.Domain.SeedWork;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface IAggregateReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}