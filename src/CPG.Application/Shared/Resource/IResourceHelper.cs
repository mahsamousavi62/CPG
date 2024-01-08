using CPG.Domain.SharedKernel;
using System.Collections.Generic;

namespace CPG.Application.Shared.Resource;

public interface IResourceHelper
{
    Result<Dictionary<string, string>> GetResources();
}
