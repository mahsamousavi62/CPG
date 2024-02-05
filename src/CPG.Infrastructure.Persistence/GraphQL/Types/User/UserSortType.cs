using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.SortInputType;
using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.Types.Company;

public class UserSortType : SortInputType<UserReadModel>
{
    protected override void Configure(ISortInputTypeDescriptor<UserReadModel> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Id).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.IDPId).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.FirstName).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.LastName).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.CreationDate).Type<AscDescSortEnumType>();
        descriptor.Field(f => f.ModificationDate).Type<AscDescSortEnumType>();
    }
}
