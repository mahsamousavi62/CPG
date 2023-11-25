using Ardalis.Specification;

namespace CPG.Domain.SharedKernel;

public interface ICommonServiceRepository<T> : IReadRepositoryBase<T> where T : class
{
}

