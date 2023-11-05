using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Redis
{
    public interface IRedisCaheService
    {
        T GetData<T>(string key);
        void SetData<T>(string key, T value);
        void RemoveKey(string key);
    }
}
