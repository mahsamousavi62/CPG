using CPG.Application.Shared.Resource;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Tests.Base;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CPG.Domain.Tests.Unit.Shared
{
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
}
