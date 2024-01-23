using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.FilterInputType;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Company;

public class CompanyFilterType : FilterInputType<CompanyReadModel>
{
    protected override void Configure(
   IFilterInputTypeDescriptor<CompanyReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.PersianName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.EnglishName).Type<CustomStringOperationFilterInputType>();
        descriptor.Field(f => f.IsActive).Type<CustomBooleanOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
    }
}
