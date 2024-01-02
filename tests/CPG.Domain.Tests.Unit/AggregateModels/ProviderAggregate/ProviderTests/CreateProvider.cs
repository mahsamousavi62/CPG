using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.Tests.Unit.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.AggregateModels.ProviderAggregate.ProviderTests;

public class CreateProvider : AggregateTestHelper
{
    private static Provider Act(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData, short verificationTimeLimit, Url ipgBaseUrl)
           => Provider.Create(persianName, englishName, providerType, logo, providerData, verificationTimeLimit, ipgBaseUrl);

    //[Fact]
    //public void given_valid_data_CPG_user_should_be_created()
    //{   
    //    var provider = Act(GetProviderPersianName, GetProviderEnglishName, ProviderType.Vandar, , GetProviderData);

    //    provider.Should().NotBeNull();
    //    provider.IsActive.Should().BeTrue();
    //}
}