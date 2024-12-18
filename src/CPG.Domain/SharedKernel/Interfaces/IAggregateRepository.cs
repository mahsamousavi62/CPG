using Ardalis.Specification;
using CPG.Domain.SeedWork;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface IAggregateRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}