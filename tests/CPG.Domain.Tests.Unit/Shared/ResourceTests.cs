using CPG.Application.Shared.Resource;
using CPG.Tests.Base;
using Xunit;

namespace CPG.Domain.Tests.Unit.Shared;

public class ResourceTests : TestBase
{
    protected readonly IResourceHelper _resourceHelper = new ResourceHelper();

    [Fact]
    private void resource_loaded_successfully()
    {
        var resources = _resourceHelper.GetResources();

        Assert.NotNull(resources);
    }            
}
