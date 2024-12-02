using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification.EntityFrameworkCore;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Infrastructure.Persistence.DbContexts;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class WriteRepository<T>(WriteDbContext writeDbContext) : RepositoryBase<T>(writeDbContext), IWriteRepository<T> where T : class
    {
    }
}
