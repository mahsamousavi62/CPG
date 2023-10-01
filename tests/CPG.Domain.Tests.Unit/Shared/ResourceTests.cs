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
        protected readonly IResourceHandler _resourceHandler = new ResourceHandler();

        [Fact]
        private void resource_loaded_successfully()
        {
            var resources = _resourceHandler.GetResources();

            Assert.NotNull(resources);
        }            
    }
}
