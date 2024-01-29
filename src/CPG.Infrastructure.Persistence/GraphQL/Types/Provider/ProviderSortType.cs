using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Provider;

public class ProviderSortType : SortInputType<ProviderReadModel>
{
    protected override void Configure(ISortInputTypeDescriptor<ProviderReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.PersianName).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.EnglishName).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.CreationDate).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.ModificationDate).Type<AscDescSortEnumType>();
    }
}