using Ardalis.Specification;

namespace CPG.Domain.SharedKernel.Interfaces
{
    public interface ICommonServiceRepository<T> : IReadRepositoryBase<T> where T : class
    {
    }
}

