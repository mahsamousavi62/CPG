using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Bank;

public class BankSortType : SortInputType<BankReadModel>
{
    protected override void Configure(ISortInputTypeDescriptor<BankReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.Name).Type<AscDescSortEnumType>();        
        descriptor.Field(f => f.CreationDate).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.ModificationDate).Type<AscDescSortEnumType>();
    }
}