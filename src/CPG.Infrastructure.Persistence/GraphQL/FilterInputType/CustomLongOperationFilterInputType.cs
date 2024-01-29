using HotChocolate.Data.Filters;
using HotChocolate.Data;
using HotChocolate.Types;

namespace CPG.Infrastructure.Persistence.GraphQL.Types;

public class CustomLongOperationFilterInputType : LongOperationFilterInputType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals).Type<LongType>();
        descriptor.Operation(DefaultFilterOperations.GreaterThan).Type<LongType>();
        descriptor.Operation(DefaultFilterOperations.GreaterThanOrEquals).Type<LongType>();
        descriptor.Operation(DefaultFilterOperations.LowerThan).Type<LongType>();
        descriptor.Operation(DefaultFilterOperations.LowerThanOrEquals).Type<LongType>();
    }
}