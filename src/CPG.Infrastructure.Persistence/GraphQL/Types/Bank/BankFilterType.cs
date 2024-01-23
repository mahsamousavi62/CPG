using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.FilterInputType;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Bank;

internal class BankFilterType : FilterInputType<BankReadModel>
{
    protected override void Configure(
   IFilterInputTypeDescriptor<BankReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Name).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.IsActive).Type<CustomBooleanOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
    }
}