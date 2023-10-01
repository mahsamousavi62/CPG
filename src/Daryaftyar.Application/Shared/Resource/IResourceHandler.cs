using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daryaftyar.Application.Shared.Resource
{
    public interface IResourceHandler
    {
        Dictionary<string, string> GetResources();
    }
}
