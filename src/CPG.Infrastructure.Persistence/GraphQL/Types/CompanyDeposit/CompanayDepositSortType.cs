using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.CompanyDeposit;

public class CompanayDepositSortType : SortInputType<CompanyDepositReadModel>
{   
    protected override void Configure(ISortInputTypeDescriptor<CompanyDepositReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.Name).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.CreationDate).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.ModificationDate).Type<AscDescSortEnumType>();
    }
}