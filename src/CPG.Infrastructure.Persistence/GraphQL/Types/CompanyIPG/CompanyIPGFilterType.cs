using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;
using CPG.Infrastructure.Persistence.GraphQL.FilterInputType;
using HotChocolate.Data.Filters;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.CompanyDeposit;

public class CompanyIPGFilterType:FilterInputType<CompanyIPGReadModel>
{
    protected override void Configure(IFilterInputTypeDescriptor<CompanyIPGReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.IsActive).Type<CustomBooleanOperationFilterInputType>();
        descriptor.Field(f => f.Id).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.CompanyId).Type<CustomLongOperationFilterInputType>();
        descriptor.Field(f => f.IPGTypeId).Type<CustomLongOperationFilterInputType>();
    }
}
