using Ardalis.Specification;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
}
