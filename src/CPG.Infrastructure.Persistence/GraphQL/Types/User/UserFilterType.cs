using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.FilterInputType;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Company;

public class UserFilterType : FilterInputType<UserReadModel>
{
    protected override void Configure(
   IFilterInputTypeDescriptor<UserReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.FirstName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.LastName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.IDPId).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.IsActive).Type<CustomBooleanOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.NationalCode).Type<CustomStringOperationFilterInputType>();

    }
}
