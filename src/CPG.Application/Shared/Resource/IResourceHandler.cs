using System.Collections.Generic;

namespace CPG.Application.Shared.Resource
{
    public interface IResourceHandler
    {
        Dictionary<string, string> GetResources();
    }
}
