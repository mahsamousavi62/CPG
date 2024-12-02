using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Infrastructure.Persistence.DbContexts;

namespace CPG.Infrastructure.Persistence.Repositories;

public class ReadRepository<T>(ReadDbContext readDbContext) : RepositoryBase<T>(readDbContext), IReadRepository<T> where T : class
{
}