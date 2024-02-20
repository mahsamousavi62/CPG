using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Types;

namespace CPG.Infrastructure.Persistence.GraphQL.CustomFilterInputType;

public class CustomIntOperationFilterInputType : IntOperationFilterInputType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals).Type<IntType>();
        descriptor.Operation(DefaultFilterOperations.GreaterThan).Type<IntType>();
        descriptor.Operation(DefaultFilterOperations.GreaterThanOrEquals).Type<IntType>();
        descriptor.Operation(DefaultFilterOperations.LowerThan).Type<IntType>();
        descriptor.Operation(DefaultFilterOperations.LowerThanOrEquals).Type<IntType>();
    }
}