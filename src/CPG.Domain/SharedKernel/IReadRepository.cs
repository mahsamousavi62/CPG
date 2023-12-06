using Ardalis.Specification;

namespace CPG.Domain.SharedKernel;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
}
