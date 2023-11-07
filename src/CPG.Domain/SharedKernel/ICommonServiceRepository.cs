using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel
{
    public interface ICommonServiceRepository<T> : IReadRepositoryBase<T> where T : class
    {
    }
}

