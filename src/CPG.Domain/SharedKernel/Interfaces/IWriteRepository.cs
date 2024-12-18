using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace CPG.Domain.SharedKernel.Interfaces
{
    public interface IWriteRepository<T> : IRepositoryBase<T> where T : class
    {
    }
}
