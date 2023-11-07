using Ardalis.Specification.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class CommonServiceRepository<T> : RepositoryBase<T> where T : class
    {
        public CommonServiceRepository(WriteDbContext writeDbContext)
            : base(writeDbContext)
        {
        }
    }
}
