using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Tests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.Helpers;

public class AggregateTestHelper : TestBase
{
    //protected CPGUser GetValidCPGUserAggregate() => PrepareCPGUserAggregate();
    //protected Book GetValidBookAggregate() => PrepareBookAggregate();

    protected string GetProviderPersianName => CreatePersianString(6);
    protected string GetProviderEnglishName => CreateString();
    protected string GetProviderData => CreateString();
}
