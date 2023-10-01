using Daryaftyar.Application.Shared.Resource;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Tests.Base;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Daryaftyar.Domain.Tests.Unit.Shared
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
