using HotChocolate.Data.Sorting;

namespace CPG.Infrastructure.Persistence.GraphQL.SortInputType;

public class AscDescSortEnumType : DefaultSortEnumType
{
    protected override void Configure(ISortEnumTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultSortOperations.Ascending);
        descriptor.Operation(DefaultSortOperations.Descending);
    }
}